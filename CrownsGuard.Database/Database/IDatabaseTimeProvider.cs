using FluentResults;

namespace CrownsGuard.Database.Database;

public interface IDatabaseTimeProvider
{
    Task<Result<DateTime>> GetServerTimeAsync();
}