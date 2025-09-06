using FluentResults;

namespace CrownsGuard.Database.Database;

internal class DatabaseTimeProvider : IDatabaseTimeProvider
{
    private readonly IDatabaseClient _databaseClient;

    public DatabaseTimeProvider(IDatabaseClient databaseClient)
    {
        _databaseClient = databaseClient;
    }

    /// <inheritdoc />
    public async Task<Result<DateTime>> GetServerTimeAsync()
    {
        try
        {
            Console.WriteLine("Getting server time");
            var result = await _databaseClient.GetServerTimeAsync();
            Console.WriteLine($"Current server time: {result}");
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to get server time: {e}");
            return Result.Fail<DateTime>(e.Message);
        }
    }
}