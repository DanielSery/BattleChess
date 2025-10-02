using FluentResults;

namespace CrownsGuard.Database.Game;

public interface IGameTurnsCollectionHandler
{
    Task<Result> InsertTurnAsync(GameTurn gameTurn, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result> RemoveTurnsOlderThanAsync(DateTime time, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameTurn>> WaitForFirstTurnAsync(string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);

    Task<Result<GameTurn>> WaitForNextTurnAsync(string turnId, string gameId, CancellationToken cancellationToken, int timeoutSeconds = 120);
}