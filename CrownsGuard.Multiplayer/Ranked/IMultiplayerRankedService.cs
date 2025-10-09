using CrownsGuard.Database.Ranked;
using FluentResults;

namespace CrownsGuard.Multiplayer.Ranked;

internal interface IMultiplayerRankedService
{
    enum WaitResult { Timeout, GameSearch, GameJoin }
    Task<(WaitResult result, RankedGame? search, RankedGameJoin? searchJoin)> WaitForGameFindOrJoinAsync(
        string gameId,
        short targetElo,
        int eloDifference,
        int timeoutSeconds,
        CancellationToken cancellationToken);

    Task<Result<RankedGame>> CreateGameSearchAsync(
        string currentPlayerId,
        short currentPlayerElo,
        int[] currentPlayerMap, 
        CancellationToken cancellationToken);

    Task<Result<RankedGameJoin>> TryToJoinGameAsync(
        string joinedGameId,
        string playerId,
        int[] myMapData,
        CancellationToken cancellationToken);

    Task DeleteGameSearchAsync(string gameId);
}