using System.Diagnostics.CodeAnalysis;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Errors;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CrownsGuard.Database.Ranked;

[SuppressMessage("ReSharper", "PossiblyMistakenUseOfCancellationToken")]
internal class RankedGamesCollectionHandler : IRankedGamesCollectionHandler
{
    private readonly IDatabaseClient _client;
    private readonly ILogger<RankedGamesCollectionHandler> _logger;

    public RankedGamesCollectionHandler(IDatabaseClient databaseClient, ILogger<RankedGamesCollectionHandler> logger)
    {
        _client = databaseClient;
        _logger = logger;
    }

    public async Task<Result> ConfirmGameJoinAsync(string gameId, string joinId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Confirming game join");
            var filter = Builders<RankedGame>.Filter.Eq(l => l.Id, gameId);
            var update = Builders<RankedGame>.Update.Set(x => x.JoinedId, joinId);
            var result = await _client.RankedGames.UpdateOneAsync(filter, update, cancellationToken: linkedSource.Token);
            return result.IsAcknowledged ? Result.Ok() : Result.Fail("Failed to delete confirm game join");
            
        }, nameof(ConfirmGameJoinAsync), _logger, cancellationToken);
    }

    public async Task<Result> DeleteGameSearchAsync(string deletedGameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Deleting game search");
            var gameSearchFilter = Builders<RankedGame>.Filter.Eq(l => l.Id, deletedGameId);
            await _client.RankedGames.DeleteManyAsync(gameSearchFilter, linkedSource.Token);
            
        }, nameof(DeleteGameSearchAsync), _logger, cancellationToken);
    }

    public async Task<Result<RankedGame>> FindForTargetEloAsync(string gameId, short targetElo, int eloDifference, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Waiting for ranked join request for game: {gameId}", gameId);

            var fromElo = targetElo - eloDifference;
            var toElo = targetElo + eloDifference;
            var streamFilter = Builders<ChangeStreamDocument<RankedGame>>.Filter.And(
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Insert),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Gt(cs => cs.FullDocument.Id, gameId),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.FullDocument.Version, GameVersion.VersionId),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Ne(cs => cs.FullDocument.JoinedId, null),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Gt(cs => cs.FullDocument.Elo, fromElo),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Lt(cs => cs.FullDocument.Elo, toElo));
            using var streamCursor = await _client.RankedGames.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            var streamResult = streamCursor.WaitForAddAsync(cancellationToken: linkedSource.Token);
        
            var filter = Builders<RankedGame>.Filter.And(
                Builders<RankedGame>.Filter.Gt(g => g.Id, gameId),
                Builders<RankedGame>.Filter.Eq(g => g.Version, GameVersion.VersionId),
                Builders<RankedGame>.Filter.Eq(g => g.JoinedId, null),
                Builders<RankedGame>.Filter.Gt(g => g.Elo, fromElo),
                Builders<RankedGame>.Filter.Lt(g => g.Elo, toElo));
            var foundResult = await _client.RankedGames.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            if (foundResult.IsSuccess) return foundResult;

            return await streamResult;
            
        }, nameof(FindForTargetEloAsync), _logger, cancellationToken);
    }

    public async Task<Result<RankedGame>> WaitForAcceptAsync(string joinedGameId, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Getting game with id: {joinedGameId}", joinedGameId);
            var streamFilter = Builders<ChangeStreamDocument<RankedGame>>.Filter.And(
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Or(
                    Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Update),
                    Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.OperationType, ChangeStreamOperationType.Delete)),
                Builders<ChangeStreamDocument<RankedGame>>.Filter.Eq(cs => cs.FullDocument.Id, joinedGameId));
            using var streamCursor = await _client.RankedGames.CreateChangeStreamCursorAsync(streamFilter, cancellationToken: linkedSource.Token);
            var streamResult = streamCursor.WaitForUpdateAsync(cancellationToken: linkedSource.Token);
        
            var filter = Builders<RankedGame>.Filter.Eq(g => g.JoinedId, joinedGameId);
            var foundResult = await _client.RankedGames.FindSingleResultAsync(filter, cancellationToken: linkedSource.Token);
            if (foundResult.IsSuccess && foundResult.Value.JoinedId is not null) return foundResult;
            if (foundResult.HasError<NoResultsFoundError>()) return foundResult;
            
            return await streamResult;
            
        }, nameof(WaitForAcceptAsync), _logger, cancellationToken);
    }

    public async Task<Result> InsertAsync(RankedGame game, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Creating game request");
            await _client.RankedGames.InsertOneAsync(game, cancellationToken: linkedSource.Token);
            
        }, nameof(InsertAsync), _logger, cancellationToken);
    }

    public async Task<Result<RankedGame>> GetClosestGameSearchAsync(int searchedElo, int maxDifference, CancellationToken cancellationToken, int timeoutSeconds)
    {
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedSource.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
        return await DatabaseHelper.ExecuteWithErrorHandling(async () =>
        {
            _logger.LogInformation("Searching for ranked game with elo: {fromElo}-{toElo}", searchedElo - maxDifference, searchedElo + maxDifference);
            return await _client.RankedGames.Aggregate()
                .Match(l => l.Version == GameVersion.VersionId && string.IsNullOrEmpty(l.JoinedId))
                .Project(lobby => new
                {
                    Lobby = lobby,
                    EloDifference = Math.Abs(lobby.Elo - searchedElo)
                })
                .SortBy(x => x.EloDifference)
                .Limit(1)
                .Project(x => x.Lobby)
                .FirstAsync(cancellationToken: linkedSource.Token);

        }, nameof(InsertAsync), _logger, cancellationToken);
    }
}
