using CrownsGuard.Core.Figures;
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
            if (!myMap.IsValid(unlockedFigures))
                return Result.Fail<(bool, RankedGame, RankedGameJoin)>("Setup has units which weren't unlocked yet");

            var random = new Random();
            var isHostStarting = random.Next(0, 1) == 1;

                var myMapData = myMap.GetIntData();
                var eloDifference = 50;
                var closestGameSearchResult = await _gameRequests.GetClosestGameSearchAsync(currentPlayer.Elo, eloDifference, cancellationToken);
                if (!closestGameSearchResult.TryGetValue(out var closestGameSearch)) closestGameSearch = null;

                while (closestGameSearch is not null && Math.Abs(closestGameSearch.Elo - currentPlayer.Elo) <= eloDifference)
                {
                    var joinResult = await TryToJoinGameAsync(closestGameSearch, currentPlayer, myMapData, cancellationToken);
                    if (joinResult.IsSuccess)
                    {
                        return Result.Ok<(bool, RankedGame, RankedGameJoin)>((false, closestGameSearch, joinResult.Value));
                    }

                    closestGameSearchResult = await _gameRequests.GetClosestGameSearchAsync(currentPlayer.Elo, eloDifference, cancellationToken);
                    if (!closestGameSearchResult.TryGetValue(out closestGameSearch)) closestGameSearch = null;
                }

                var createdGameSearchResult = await CreateGameSearchAsync(myMapData, currentPlayer, isHostStarting, cancellationToken);
                if (!createdGameSearchResult.TryGetValue(out var createdGameSearch)) return createdGameSearchResult.ToResult();
                try
                {
                    while (true)
                    {
                        var (waitResult, foundSearch, foundSearchJoin) = await WaitForLobbyOrJoinAsync(createdGameSearch.Id, currentPlayer.Elo, eloDifference, 20, cancellationToken);
                        if (waitResult == WaitResult.GameJoin)
                        {
                            var confirmationResult = await _gameRequests.ConfirmGameAsync(createdGameSearch.Id, foundSearchJoin!.Id, cancellationToken);
                            if (confirmationResult.IsSuccess)
                            {
                                return Result.Ok((true, createdGameSearch, foundSearchJoin));
                            }
                        }
                        else if (waitResult == WaitResult.GameSearch)
                        {
                            var joinResult = await TryToJoinGameAsync(foundSearch!, currentPlayer, myMapData, cancellationToken);
                            if (joinResult.IsSuccess)
                            {
                                return Result.Ok<(bool, RankedGame, RankedGameJoin)>((false, foundSearch!, joinResult.Value));
                            }
                        }
                        else if (eloDifference < 300)
                        {
                            Console.WriteLine($"No game found with elo difference {eloDifference}, increasing to {eloDifference + 50}");
                            eloDifference += 50;
                        }
                        else
                        {
                            Console.WriteLine($"No game found with elo difference {eloDifference}, continuing search");
                        }
                    }
                }
                finally
                {
                    if (string.IsNullOrEmpty(createdGameSearch.JoinedId))
                    {
                        await DeleteGameSearch(createdGameSearch);
                    }
                }
    }

    private async Task DeleteGameSearch(RankedGame deletedGame)
    {
        await _gameRequests.DeleteGameSearchAsync(deletedGame.Id, CancellationToken.None);
        await _gameJoins.DeleteGameJoinsAsync(deletedGame.Id, CancellationToken.None);
    }

    private enum WaitResult
    {
        Timeout,
        GameSearch,
        GameJoin
    }
    
    private async Task<(WaitResult result, RankedGame? search, RankedGameJoin? searchJoin)> WaitForLobbyOrJoinAsync(
        string gameId,
        short targetElo,
        int eloDifference,
        int timeoutSeconds,
        CancellationToken cancellationToken)
    {
        var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutTokenSource.Token);

        var gameSearch = Task.Run(async () => await _gameRequests.FindGameForTargetEloAsync(gameId, targetElo, eloDifference, cancellationTokenSource.Token), cancellationTokenSource.Token);
        var joinTask = Task.Run(async () => await _gameJoins.WaitForGameJoinAsync(gameId, cancellationTokenSource.Token), cancellationTokenSource.Token);

        var completedTask = await Task.WhenAny(gameSearch, joinTask, Task.Delay(TimeSpan.FromSeconds(timeoutSeconds), cancellationTokenSource.Token));
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

        var result = await _gameRequests.InsertAsync(game, cancellationToken);
        if (result.IsFailed) return result;
        return game;
    }

    private async Task<Result<RankedGameJoin>> TryToJoinGameAsync(
        RankedGame joinedGame, 
        RegisteredPlayer currentPlayer, 
        int[] myMapData,
        CancellationToken cancellationToken)
    {
        var gameJoin = new RankedGameJoin()
        {
            GameId = joinedGame.Id,
            PlayerId = currentPlayer.Id,
            Map = myMapData,
        };
        var insertResult = await _gameJoins.InsertGameJoinAsync(gameJoin, cancellationToken);
        if (insertResult.IsFailed) return Result.Fail("Failed to insert game join");

        Console.WriteLine("Waiting for join request confirmation");
        var updatedJoinedGameResult = await _gameRequests.WaitForGameAcceptAsync(joinedGame.Id, cancellationToken, 20);
        if (!updatedJoinedGameResult.TryGetValue(out var updatedJoinedGame))
        {
            await DeleteGameSearch(joinedGame);
            Console.WriteLine("Invalid game search");
            return Result.Fail("Joining timed out");
        }
        else if (updatedJoinedGame.JoinedId == gameJoin.Id)
        {
            await DeleteGameSearch(updatedJoinedGame);
            Console.WriteLine($"Confirmed join request with id: {gameJoin.Id}");
            return Result.Ok(gameJoin);
        }
        else
        {
            Console.WriteLine("The lobby is already full");
            return Result.Fail("The lobby is already full");
        }
    }

}