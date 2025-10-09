using FluentResults;
using Microsoft.Extensions.Logging;

namespace CrownsGuard.Database.Database;

internal class DatabaseTimeProvider : IDatabaseTimeProvider
{
    private readonly IDatabaseClient _databaseClient;
    private readonly ILogger<DatabaseTimeProvider> _logger;

    public DatabaseTimeProvider(
        IDatabaseClient databaseClient,
        ILogger<DatabaseTimeProvider> logger)
    {
        _databaseClient = databaseClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<DateTime>> GetServerTimeAsync()
    {
        try
        {
            _logger.LogInformation("Getting server time");
            var result = await _databaseClient.GetServerTimeAsync();
            _logger.LogInformation("Current server time: {result}", result);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to get server time: {e}", e);
            return Result.Fail<DateTime>(e.Message);
        }
    }
}