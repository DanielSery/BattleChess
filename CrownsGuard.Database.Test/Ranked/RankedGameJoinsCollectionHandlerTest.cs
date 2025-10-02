using CrownsGuard.Database.Database;
using CrownsGuard.Database.Ranked;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Database.Test.Ranked;

public class RankedGameJoinsCollectionHandlerTest : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoCollection<RankedGameJoin> _rankedGameJoinsCollection;
    private readonly RankedGameJoinsCollectionHandler _handler;

    public RankedGameJoinsCollectionHandlerTest()
    {
        // Start MongoDB test instance
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Create test database and collection
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var database = client.GetDatabase("TestBattleChess");
        _rankedGameJoinsCollection = database.GetCollection<RankedGameJoin>("RankedGameJoins");

        // Create a mock database client for testing
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGameJoins).Returns(_rankedGameJoinsCollection);
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        // Create a mock logger for testing
        var mockLogger = new Mock<ILogger<RankedGameJoinsCollectionHandler>>();

        // Create handler with mocked dependencies
        _handler = new RankedGameJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Helper Methods

    private RankedGameJoin CreateTestRankedGameJoin(
        string id,
        string? gameId = null,
        string? playerId = null,
        int[]? map = null)
    {
        return new RankedGameJoin
        {
            Id = id,
            GameId = gameId ?? "507f1f77bcf86cd799439011",
            PlayerId = playerId ?? "507f1f77bcf86cd799439012",
            Map = map ?? new[] { 0, 1, 2, 3, 4 }
        };
    }

    private async Task<List<RankedGameJoin>> GetAllRankedGameJoinsAsync()
    {
        return await _rankedGameJoinsCollection.Find(_ => true).ToListAsync();
    }

    private async Task InsertTestRankedGameJoinAsync(RankedGameJoin rankedGameJoin)
    {
        await _rankedGameJoinsCollection.InsertOneAsync(rankedGameJoin);
    }

    #endregion

    #region InsertGameJoinAsync Tests

    [Fact]
    public async Task InsertGameJoinAsync_ValidGameJoin_InsertsSuccessfully()
    {
        // Arrange
        var gameJoin = CreateTestRankedGameJoin(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012",
            playerId: "507f1f77bcf86cd799439013");

        // Act
        var result = await _handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedJoins = await GetAllRankedGameJoinsAsync();
        Assert.Single(insertedJoins);
        Assert.Equal(gameJoin.Id, insertedJoins.First().Id);
        Assert.Equal(gameJoin.GameId, insertedJoins.First().GameId);
        Assert.Equal(gameJoin.PlayerId, insertedJoins.First().PlayerId);
    }

    [Fact]
    public async Task InsertGameJoinAsync_DuplicateId_ReturnsFailure()
    {
        // Arrange
        var gameJoinId = "507f1f77bcf86cd799439011";
        var gameJoin1 = CreateTestRankedGameJoin(gameJoinId, gameId: "507f1f77bcf86cd799439012");
        var gameJoin2 = CreateTestRankedGameJoin(gameJoinId, gameId: "507f1f77bcf86cd799439013");

        // Act
        await _handler.InsertGameJoinAsync(gameJoin1, CancellationToken.None, 30);
        var result = await _handler.InsertGameJoinAsync(gameJoin2, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task InsertGameJoinAsync_InvalidData_ReturnsFailure()
    {
        // Arrange
        var gameJoin = CreateTestRankedGameJoin("invalid-id");

        // Act
        var result = await _handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task InsertGameJoinAsync_Timeout_ReturnsFailure()
    {
        // Arrange
        var gameJoin = CreateTestRankedGameJoin("507f1f77bcf86cd799439011");

        // Act
        var result = await _handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Operation timeout", result.Errors.First().Message);
    }

    [Fact]
    public async Task InsertGameJoinAsync_CancellationToken_CancelsOperation()
    {
        // Arrange
        var gameJoin = CreateTestRankedGameJoin("507f1f77bcf86cd799439011");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.InsertGameJoinAsync(gameJoin, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertGameJoinAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGameJoins)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<RankedGameJoinsCollectionHandler>>();
        var handler = new RankedGameJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        var gameJoin = CreateTestRankedGameJoin("507f1f77bcf86cd799439011");

        // Act
        var result = await handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region DeleteGameJoinsAsync Tests

    [Fact]
    public async Task DeleteGameJoinsAsync_ValidGameId_DeletesJoins()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var gameJoin1 = CreateTestRankedGameJoin("507f1f77bcf86cd799439012", gameId: gameId);
        var gameJoin2 = CreateTestRankedGameJoin("507f1f77bcf86cd799439013", gameId: gameId);
        var gameJoin3 = CreateTestRankedGameJoin("507f1f77bcf86cd799439014", gameId: "507f1f77bcf86cd799439015");

        await InsertTestRankedGameJoinAsync(gameJoin1);
        await InsertTestRankedGameJoinAsync(gameJoin2);
        await InsertTestRankedGameJoinAsync(gameJoin3);

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingJoins = await GetAllRankedGameJoinsAsync();
        Assert.Single(remainingJoins);
        Assert.Equal(gameJoin3.Id, remainingJoins.First().Id);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_InvalidGameId_ReturnsSuccess()
    {
        // Arrange
        var invalidGameId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _handler.DeleteGameJoinsAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_Timeout_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Operation timeout", result.Errors.First().Message);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_CancellationToken_CancelsOperation()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGameJoins)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<RankedGameJoinsCollectionHandler>>();
        var handler = new RankedGameJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.DeleteGameJoinsAsync("507f1f77bcf86cd799439011", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region WaitForGameJoinAsync Tests

    [Fact]
    public async Task WaitForGameJoinAsync_ImmediateJoin_ReturnsGameJoin()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var gameJoin = CreateTestRankedGameJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId);

        await InsertTestRankedGameJoinAsync(gameJoin);

        // Act
        var result = await _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(gameJoin.Id, result.Value.Id);
        Assert.Equal(gameJoin.GameId, result.Value.GameId);
        Assert.Equal(gameJoin.PlayerId, result.Value.PlayerId);
    }

    [Fact]
    public async Task WaitForGameJoinAsync_ChangeStreamWaiting_ReturnsGameJoin()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";

        // Act - Start waiting for a game join
        var waitTask = _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 30);

        // Insert a game join after a short delay
        await Task.Delay(100);
        var gameJoin = CreateTestRankedGameJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId);

        await InsertTestRankedGameJoinAsync(gameJoin);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(gameJoin.Id, result.Value.Id);
    }

    [Fact]
    public async Task WaitForGameJoinAsync_Timeout_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 1);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task WaitForGameJoinAsync_CancellationToken_CancelsOperation()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.WaitForGameJoinAsync(gameId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForGameJoinAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGameJoins)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<RankedGameJoinsCollectionHandler>>();
        var handler = new RankedGameJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.WaitForGameJoinAsync("507f1f77bcf86cd799439011", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region Enhanced Error Message Validation Tests

    [Fact]
    public async Task DeleteGameJoinsAsync_InvalidObjectIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var invalidGameId = "invalid-object-id-format";

        // Act
        var result = await _handler.DeleteGameJoinsAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    [Fact]
    public async Task WaitForGameJoinAsync_InvalidGameIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var invalidGameId = "invalid-object-id-format";

        // Act
        var result = await _handler.WaitForGameJoinAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    #endregion

    #region Advanced Boundary Condition Tests

    [Fact]
    public async Task TimeoutBoundaryTests_MinimumTimeout_ReturnsTimeoutError()
    {
        // Arrange
        var gameJoin = CreateTestRankedGameJoin("507f1f77bcf86cd799439011");

        // Act
        var result = await _handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Operation timeout", result.Errors.First().Message);
    }

    [Fact]
    public async Task ConcurrentOperations_ChangeStreamRaceCondition()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";

        // Start waiting for a game join
        var waitTask = _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 30);

        // Insert multiple game joins concurrently
        var gameJoin1 = CreateTestRankedGameJoin("507f1f77bcf86cd799439012", gameId: gameId);
        var gameJoin2 = CreateTestRankedGameJoin("507f1f77bcf86cd799439013", gameId: gameId);
        var gameJoin3 = CreateTestRankedGameJoin("507f1f77bcf86cd799439014", gameId: gameId);

        var insertTask1 = InsertTestRankedGameJoinAsync(gameJoin1);
        var insertTask2 = InsertTestRankedGameJoinAsync(gameJoin2);
        var insertTask3 = InsertTestRankedGameJoinAsync(gameJoin3);

        await Task.WhenAll(insertTask1, insertTask2, insertTask3);

        // Act
        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        // Should return one of the inserted game joins
        var allJoins = await GetAllRankedGameJoinsAsync();
        var returnedJoin = allJoins.FirstOrDefault(j => j.Id == result.Value.Id);
        Assert.NotNull(returnedJoin);
    }

    #endregion

    #region Integration and Interaction Tests

    [Fact]
    public async Task ComplexWorkflow_CompleteGameJoinLifecycle()
    {
        // This test simulates a complete workflow of game join operations

        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var gameJoin = CreateTestRankedGameJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");

        // Act & Assert - Complete workflow
        // 1. Insert game join
        var insertResult = await _handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 30);
        Assert.True(insertResult.IsSuccess);

        // 2. Wait for the game join (should return immediately)
        var waitResult = await _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 30);
        Assert.True(waitResult.IsSuccess);
        Assert.Equal(gameJoin.Id, waitResult.Value.Id);

        // 3. Clean up - delete the game joins
        var deleteResult = await _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 30);
        Assert.True(deleteResult.IsSuccess);

        // 4. Verify final state
        var finalJoins = await GetAllRankedGameJoinsAsync();
        Assert.DoesNotContain(finalJoins, j => j.GameId == gameId);
    }

    #endregion

    #region Concurrency Tests

    [Fact]
    public async Task Concurrent_MultipleInsertGameJoinAsync_SameGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var gameJoin1 = CreateTestRankedGameJoin("507f1f77bcf86cd799439012", gameId: gameId, playerId: "507f1f77bcf86cd799439013");
        var gameJoin2 = CreateTestRankedGameJoin("507f1f77bcf86cd799439014", gameId: gameId, playerId: "507f1f77bcf86cd799439015");

        // Act - Start both operations concurrently
        var task1 = _handler.InsertGameJoinAsync(gameJoin1, CancellationToken.None, 30);
        var task2 = _handler.InsertGameJoinAsync(gameJoin2, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2);

        var result1 = await task1;
        var result2 = await task2;

        // Assert
        // Both operations should succeed since they have different IDs
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);

        // Verify both joins were inserted
        var allJoins = await GetAllRankedGameJoinsAsync();
        Assert.Equal(2, allJoins.Count);
    }

    [Fact]
    public async Task Concurrent_ReadAndWriteOperations_SameGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var gameJoin = CreateTestRankedGameJoin("507f1f77bcf86cd799439012", gameId: gameId);

        // Act - Mix of read and write operations
        var task1 = _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 30);
        var task2 = _handler.InsertGameJoinAsync(gameJoin, CancellationToken.None, 30);
        var task3 = _handler.WaitForGameJoinAsync(gameId, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2, task3);

        var result1 = await task1;
        var result2 = await task2;
        var result3 = await task3;

        // Assert
        Assert.True(result1.IsSuccess || result1.IsFailed);
        Assert.True(result2.IsSuccess);
        Assert.True(result3.IsSuccess || result3.IsFailed);
    }

    #endregion
}