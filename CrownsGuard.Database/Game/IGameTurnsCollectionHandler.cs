using FluentResults;

namespace CrownsGuard.Database.Game;

public interface IGameTurnsCollectionHandler
{
    Task<Result> InsertAsync(GameTurn gameTurn, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> RemoveOlderThanAsync(DateTime time, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);
}