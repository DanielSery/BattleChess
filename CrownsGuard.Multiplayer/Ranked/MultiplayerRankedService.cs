using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Players;
using CrownsGuard.Database.Ranked;
using CrownsGuard.Database.Utilities;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;

namespace CrownsGuard.Multiplayer.Ranked;

internal class MultiplayerRankedService : IMultiplayerRankedService
{
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly IRankedGameJoinsCollectionHandler _gameJoins;
    private readonly IRankedGamesCollectionHandler _gameRequests;

    public MultiplayerRankedService(
        IMultiplayerPlayerService multiplayerPlayerService,
        IRankedGameJoinsCollectionHandler gameJoins,
        IRankedGamesCollectionHandler gameRequests)
    {
        _multiplayerPlayerService = multiplayerPlayerService;
        _gameRequests = gameRequests;
        _gameJoins = gameJoins;
    }

    public async Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> FindRankedGameAsync(Figure[] myMap, CancellationToken cancellationToken)
    {
        var currentPlayer = _multiplayerPlayerService.LoggedInPlayer;
        if (currentPlayer is null)
        {
            return Result.Fail<(bool, RankedGame, RankedGameJoin)>("No player logged in");
        }

        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer!.UnlockedFigures;
        var setupValidation = myMap.ValidateResult(unlockedFigures);
        if (setupValidation.IsFailed) return setupValidation;

        var random = new Random();
        var isHostStarting = random.Next(0, 2) == 1;

        var myMapData = myMap.GetIntData();
        var eloDifference = 50;
        
        var closestGameSearchResult = await _gameRequests.GetClosestGameSearchAsync(currentPlayer.Elo, cancellationToken);
        if (!closestGameSearchResult.TryGetValue(out var closestGameSearch)) closestGameSearch = null;

        var closeGameResult = await TryJoinCloseGameAsync(closestGameSearch, currentPlayer, eloDifference, myMapData, cancellationToken);
        if (closeGameResult.IsSuccess) return closeGameResult;

        var createdGameSearchResult = await CreateGameSearchAsync(myMapData, currentPlayer, isHostStarting, cancellationToken);
        if (!createdGameSearchResult.TryGetValue(out var createdGameSearch)) return createdGameSearchResult.ToResult();
        try
        {
            return await IterativeGameSearchAsync(createdGameSearch, currentPlayer.Id, currentPlayer.Elo, eloDifference, myMapData, cancellationToken);
        }
        finally
        {
            if (string.IsNullOrEmpty(createdGameSearch.JoinedId))
            {
                await DeleteGameSearch(createdGameSearch.Id);
            }
        }
    }

    private async Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> IterativeGameSearchAsync(
        RankedGame createdGameSearch, string currentPlayerId, short currentPlayerElo, int eloDifference, int[] myMapData, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var (waitResult, foundSearch, foundSearchJoin) = await WaitForGameSearchOrJoinAsync(createdGameSearch.Id, currentPlayerElo, eloDifference, 20, cancellationToken);
            switch (waitResult)
            {
                case WaitResult.GameJoin:
                {
                    var confirmationResult = await _gameRequests.ConfirmGameJoinAsync(createdGameSearch.Id, foundSearchJoin!.Id, cancellationToken);
                    if (confirmationResult.IsSuccess) return Result.Ok((true, createdGameSearch, foundSearchJoin));
                    break;
                }
                case WaitResult.GameSearch:
                {
                    var joinResult = await TryToJoinGameAsync(foundSearch!.Id, currentPlayerId, myMapData, cancellationToken);
                    if (joinResult.IsSuccess) return Result.Ok((false, foundSearch, joinResult.Value));
                    break;
                }
                default:
                {
                    if (eloDifference < 300)
                    {
                        Console.WriteLine($"No game found with elo difference {eloDifference}, increasing to {eloDifference + 50}");
                        eloDifference += 50;
                        break;
                    }

                    Console.WriteLine($"No game found with elo difference {eloDifference}, continuing search");
                    break;
                }
            }
        }

        return Result.Fail(CancelledError.Instance);
    }

    private async Task<Result<(bool isHost, RankedGame gameSearch, RankedGameJoin gameSearchJoin)>> TryJoinCloseGameAsync(RankedGame? closestGameSearch,
        RegisteredPlayer currentPlayer, int eloDifference, int[] myMapData, CancellationToken cancellationToken)
    {
        while (closestGameSearch is not null && Math.Abs(closestGameSearch.Elo - currentPlayer.Elo) <= eloDifference)
        {
            var joinResult = await TryToJoinGameAsync(closestGameSearch.Id, currentPlayer.Id, myMapData, cancellationToken);
            if (joinResult.IsSuccess)
            {
                return Result.Ok<(bool, RankedGame, RankedGameJoin)>((false, closestGameSearch, joinResult.Value));
            }

            var closestGameSearchResult = await _gameRequests.GetClosestGameSearchAsync(currentPlayer.Elo, cancellationToken);
            if (!closestGameSearchResult.TryGetValue(out closestGameSearch)) closestGameSearch = null;
        }

        return Result.Fail("Did not find close game");
    }

    internal enum WaitResult { Timeout, GameSearch, GameJoin }
    internal async Task<(WaitResult result, RankedGame? search, RankedGameJoin? searchJoin)> WaitForGameSearchOrJoinAsync(
        string gameId,
        short targetElo,
        int eloDifference,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

        var gameSearch = _gameRequests.FindGameForTargetEloAsync(gameId, targetElo, eloDifference, cancellationTokenSource.Token);
        var joinTask = _gameJoins.WaitForGameJoinAsync(gameId, cancellationTokenSource.Token);

        var completedTask = await Task.WhenAny(gameSearch, joinTask);
        if (completedTask.IsCanceled || completedTask.IsFaulted)
            return (WaitResult.Timeout, null, null);
        
        if (completedTask == gameSearch && gameSearch.Result.IsSuccess)
        {
            await cancellationTokenSource.CancelAsync(); // stop the other watch
            return (WaitResult.GameSearch, gameSearch.Result.Value, null);
        }
        
        if (completedTask == joinTask && joinTask.Result.IsSuccess)
        {
            await cancellationTokenSource.CancelAsync(); // stop the other watch
            return (WaitResult.GameJoin, null, joinTask.Result.Value);
        }

        await cancellationTokenSource.CancelAsync(); // stop the other watch
        return (WaitResult.Timeout, null, null);
    }

    private async Task<Result<RankedGame>> CreateGameSearchAsync(int[] myMapData, RegisteredPlayer currentPlayer, bool isHostStarting, CancellationToken cancellationToken)
    {
        var game = new RankedGame()
        {
            Map = myMapData,
            PlayerId = currentPlayer.Id,
            Elo = currentPlayer.Elo,
            Version = GameVersion.VersionId,
            IsHostStarting = isHostStarting,
        };

        var result = await _gameRequests.InsertGameAsync(game, cancellationToken);
        if (result.IsFailed) return result;
        return game;
    }

    internal async Task<Result<RankedGameJoin>> TryToJoinGameAsync(
        string joinedGameId, 
        string playerId, 
        int[] myMapData,
        CancellationToken cancellationToken)
    {
        var gameJoin = new RankedGameJoin()
        {
            GameId = joinedGameId,
            PlayerId = playerId,
            Map = myMapData,
        };
        var insertResult = await _gameJoins.InsertGameJoinAsync(gameJoin, cancellationToken);
        if (insertResult.IsFailed) return Result.Fail("Failed to insert game join");

        var confirmationResult = await _gameRequests.WaitForGameConfirmationAsync(joinedGameId, cancellationToken, 20);
        if (!confirmationResult.TryGetValue(out var confirmedGame)) return confirmationResult.ToResult();

        if (confirmedGame.JoinedId == null)
        {
            await DeleteGameSearch(joinedGameId);
            return Result.Fail("The ranked game was invalid");
        }

        if (confirmedGame.JoinedId == gameJoin.Id)
        {
            await DeleteGameSearch(joinedGameId);
            return Result.Ok(gameJoin);
        }

        return Result.Fail("The lobby is already full");
    }

    private async Task DeleteGameSearch(string gameId)
    {
        await _gameRequests.DeleteGameSearchAsync(gameId, CancellationToken.None);
        await _gameJoins.DeleteGameJoinsAsync(gameId, CancellationToken.None);
    }

}