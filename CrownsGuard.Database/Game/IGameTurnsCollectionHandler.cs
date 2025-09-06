using FluentResults;

namespace CrownsGuard.Database.Game;

public interface IGameTurnsCollectionHandler
{
    Task<Result> InsertAsync(GameTurn gameTurn, CancellationToken cancellationToken);

    Task<Result> RemoveOlderThanAsync(DateTime time, CancellationToken cancellationToken);

    Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, TimeSpan timeout);

    Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, TimeSpan timeout);
}