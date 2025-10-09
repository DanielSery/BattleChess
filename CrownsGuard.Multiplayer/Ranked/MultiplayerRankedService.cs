using CrownsGuard.Database.Ranked;
using CrownsGuard.Database.Utilities;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;

namespace CrownsGuard.Multiplayer.Ranked;

internal class MultiplayerRankedService : IMultiplayerRankedService
{
    private readonly IRankedGameJoinsCollectionHandler _gameJoins;
    private readonly IRankedGamesCollectionHandler _gameRequests;

    public MultiplayerRankedService(
        IRankedGameJoinsCollectionHandler gameJoins,
        IRankedGamesCollectionHandler gameRequests)
    {
        _gameRequests = gameRequests;
        _gameJoins = gameJoins;
    }

    public async Task<(IMultiplayerRankedService.WaitResult result, RankedGame? search, RankedGameJoin? searchJoin)> WaitForGameFindOrJoinAsync(
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
            return (IMultiplayerRankedService.WaitResult.Timeout, null, null);
        
        if (completedTask == gameSearch && gameSearch.Result.IsSuccess)
        {
            await cancellationTokenSource.CancelAsync(); // stop the other watch
            return (IMultiplayerRankedService.WaitResult.GameSearch, gameSearch.Result.Value, null);
        }
        
        if (completedTask == joinTask && joinTask.Result.IsSuccess)
        {
            await cancellationTokenSource.CancelAsync(); // stop the other watch
            return (IMultiplayerRankedService.WaitResult.GameJoin, null, joinTask.Result.Value);
        }

        await cancellationTokenSource.CancelAsync(); // stop the other watch
        return (IMultiplayerRankedService.WaitResult.Timeout, null, null);
    }

    public async Task<Result<RankedGame>> CreateGameSearchAsync(
        string currentPlayerId,
        short currentPlayerElo,
        int[] currentPlayerMap, 
        CancellationToken cancellationToken)
    {
        var random = new Random();
        var isHostStarting = random.Next(0, 1) == 1;
        
        var game = new RankedGame()
        {
            Map = currentPlayerMap,
            PlayerId = currentPlayerId,
            Elo = currentPlayerElo,
            Version = GameVersion.VersionId,
            IsHostStarting = isHostStarting,
        };

        var result = await _gameRequests.InsertGameAsync(game, cancellationToken);
        if (result.IsFailed) return result;
        return game;
    }

    public async Task<Result<RankedGameJoin>> TryToJoinGameAsync(
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
            await DeleteGameSearchAsync(joinedGameId);
            return Result.Fail("The ranked game was invalid");
        }

        if (confirmedGame.JoinedId == gameJoin.Id)
        {
            await DeleteGameSearchAsync(joinedGameId);
            return Result.Ok(gameJoin);
        }

        return Result.Fail("The lobby is already full");
    }

    public async Task DeleteGameSearchAsync(string gameId)
    {
        await _gameRequests.DeleteGameSearchAsync(gameId, CancellationToken.None);
        await _gameJoins.DeleteGameJoinsAsync(gameId, CancellationToken.None);
    }
}