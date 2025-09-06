using FluentResults;

namespace CrownsGuard.Multiplayer.DatabaseAccess;

public interface IDatabaseTimeProvider
{
    Task<Result<DateTime>> GetServerTimeAsync();
}