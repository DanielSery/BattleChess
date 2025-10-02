using CrownsGuard.Database.Database;
using CrownsGuard.Database.Game;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Database.Test.Game;

public class GameTurnsCollectionHandlerTest : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoCollection<GameTurn> _gameTurnsCollection;
    private readonly GameTurnsCollectionHandler _handler;

    public GameTurnsCollectionHandlerTest()
    {
        // Start MongoDB test instance
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Create test database and collection
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var database = client.GetDatabase("TestBattleChess");
        _gameTurnsCollection = database.GetCollection<GameTurn>("GameTurns");

        // Create a mock database client for testing
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.GameTurns).Returns(_gameTurnsCollection);
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        // Create a mock logger for testing
        var mockLogger = new Mock<ILogger<GameTurnsCollectionHandler>>();

        // Create handler with mocked dependencies
        _handler = new GameTurnsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Helper Methods

    private GameTurn CreateTestGameTurn(
        string id,
        string gameId,
        DateTime? createdAt = null,
        double timeSpentInSeconds = 1.5,
        byte fromIndex = 0,
        byte toIndex = 1)
    {
        return new GameTurn
        {
            Id = id,
            GameId = gameId,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            TimeSpentInSeconds = timeSpentInSeconds,
            FromIndex = fromIndex,
            ToIndex = toIndex
        };
    }

    private async Task<List<GameTurn>> GetAllGameTurnsAsync()
    {
        return await _gameTurnsCollection.Find(_ => true).ToListAsync();
    }

    private async Task InsertTestGameTurnAsync(GameTurn gameTurn)
    {
        await _gameTurnsCollection.InsertOneAsync(gameTurn);
    }

    #endregion

    #region InsertAsync Tests

    [Fact]
    public async Task InsertAsync_ValidGameTurn_ReturnsSuccess()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012");

        // Act
        var result = await _handler.InsertTurnAsync(gameTurn, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedTurns = await GetAllGameTurnsAsync();
        Assert.Single(insertedTurns);
        Assert.Equal(gameTurn.Id, insertedTurns.First().Id);
        Assert.Equal(gameTurn.GameId, insertedTurns.First().GameId);
    }

    [Fact]
    public async Task InsertAsync_MultipleGameTurns_AllInsertedSuccessfully()
    {
        // Arrange
        var gameTurns = new[]
        {
            CreateTestGameTurn(
                id: "507f1f77bcf86cd799439011",
                gameId: "507f1f77bcf86cd799439012"),
            CreateTestGameTurn(
                id: "507f1f77bcf86cd799439012",
                gameId: "507f1f77bcf86cd799439012"),
            CreateTestGameTurn(
                id: "507f1f77bcf86cd799439013",
                gameId: "507f1f77bcf86cd799439013")
        };

        // Act
        foreach (var gameTurn in gameTurns)
        {
            var result = await _handler.InsertTurnAsync(gameTurn, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        // Assert
        var insertedTurns = await GetAllGameTurnsAsync();
        Assert.Equal(3, insertedTurns.Count);
    }

    [Fact]
    public async Task InsertAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn("turn0", "game0");

        // Act
        var result = await _handler.InsertTurnAsync(gameTurn, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn("turn0", "game0");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.InsertTurnAsync(gameTurn, cancellationTokenSource.Token);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn("turn0", "game0");
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.GameTurns)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<GameTurnsCollectionHandler>>();
        var handler = new GameTurnsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.InsertTurnAsync(gameTurn, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region RemoveOlderThanAsync Tests

    [Fact]
    public async Task RemoveOlderThanAsync_ValidTime_RemovesOldTurns()
    {
        // Arrange
        var oldTime = DateTime.UtcNow.AddMinutes(-10);
        var recentTime = DateTime.UtcNow.AddMinutes(-1);

        var oldTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012",
            createdAt: oldTime);
        var recentTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439012",
            gameId: "507f1f77bcf86cd799439012",
            createdAt: recentTime);

        await InsertTestGameTurnAsync(oldTurn);
        await InsertTestGameTurnAsync(recentTurn);

        // Act
        var timeoutTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var result = await _handler.RemoveTurnsOlderThanAsync(DateTime.UtcNow.AddMinutes(-5), timeoutTokenSource.Token);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingTurns = await GetAllGameTurnsAsync();
        Assert.Single(remainingTurns);
        Assert.Equal(recentTurn.Id, remainingTurns.First().Id);
    }

    [Fact]
    public async Task RemoveOlderThanAsync_NoOldTurns_NothingRemoved()
    {
        // Arrange
        var recentTime = DateTime.UtcNow.AddMinutes(-1);
        var recentTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012",
            createdAt: recentTime);

        await InsertTestGameTurnAsync(recentTurn);

        // Act
        var result = await _handler.RemoveTurnsOlderThanAsync(DateTime.UtcNow.AddMinutes(-10), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingTurns = await GetAllGameTurnsAsync();
        Assert.Single(remainingTurns);
        Assert.Equal(recentTurn.Id, remainingTurns.First().Id);
    }

    [Fact]
    public async Task RemoveOlderThanAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.RemoveTurnsOlderThanAsync(DateTime.UtcNow, cancellationTokenSource.Token);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RemoveOlderThanAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange

        // Act
        var result = await _handler.RemoveTurnsOlderThanAsync(DateTime.UtcNow, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region WaitForFirstTurnAsync Tests

    [Fact]
    public async Task WaitForFirstTurnAsync_TurnExists_ReturnsTurn()
    {
        // Arrange
        var gameId = ObjectId.GenerateNewId().ToString();
        var gameTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439011",
            gameId: gameId);

        // Insert turn first
        await InsertTestGameTurnAsync(gameTurn);

        // Act
        var result = await _handler.WaitForFirstTurnAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(gameTurn.Id, result.Value.Id);
        Assert.Equal(gameTurn.GameId, result.Value.GameId);
    }

    [Fact]
    public async Task WaitForFirstTurnAsync_TurnDoesNotExistWithinCancel_ReturnsFailure()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var gameId = ObjectId.GenerateNewId().ToString();

        // Act
        var result = await _handler.WaitForFirstTurnAsync(gameId, cts.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancel", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForFirstTurnAsync_TurnDoesNotExistWithinTimeout_ReturnsFailure()
    {
        // Arrange
        var gameId = ObjectId.GenerateNewId().ToString();

        // Act
        var result = await _handler.WaitForFirstTurnAsync(gameId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region WaitForNextTurnAsync Tests

    [Fact]
    public async Task WaitForNextTurnAsync_NextTurnExists_ReturnsNextTurn()
    {
        // Arrange
        var gameId = ObjectId.GenerateNewId().ToString();
        var firstTurnId = ObjectId.GenerateNewId().ToString();
        var secondTurnId = ObjectId.GenerateNewId().ToString();

        var firstTurn = CreateTestGameTurn(id: firstTurnId, gameId: gameId);
        var secondTurn = CreateTestGameTurn(id: secondTurnId, gameId: gameId);

        await InsertTestGameTurnAsync(firstTurn);
        await InsertTestGameTurnAsync(secondTurn);

        // Act
        var result = await _handler.WaitForNextTurnAsync(firstTurnId, gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(secondTurn.Id, result.Value.Id);
        Assert.Equal(secondTurn.GameId, result.Value.GameId);
    }

    [Fact]
    public async Task WaitForNextTurnAsync_NoNextTurnWithinCancel_ReturnsFailure()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();
        var gameId = ObjectId.GenerateNewId().ToString();
        var turnId = ObjectId.GenerateNewId().ToString();

        var existingTurn = CreateTestGameTurn(id: turnId, gameId: gameId);
        await InsertTestGameTurnAsync(existingTurn);

        // Act
        var result = await _handler.WaitForNextTurnAsync(turnId, gameId, cts.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancel", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForNextTurnAsync_NoNextTurnWithinTimeout_ReturnsFailure()
    {
        // Arrange
        var gameId = ObjectId.GenerateNewId().ToString();
        var turnId = ObjectId.GenerateNewId().ToString();

        var existingTurn = CreateTestGameTurn(id: turnId, gameId: gameId);
        await InsertTestGameTurnAsync(existingTurn);

        // Act
        var result = await _handler.WaitForNextTurnAsync(turnId, gameId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForNextTurnAsync_InvalidTurnId_ThrowsException()
    {
        // Arrange
        var gameId = ObjectId.GenerateNewId().ToString();
        var invalidTurnId = "invalid-object-id";

        // Act
        var result = await _handler.WaitForNextTurnAsync(invalidTurnId, gameId, CancellationToken.None, 30);
        
        // Assert
        Assert.True(result.IsFailed);
    }

    #endregion

    #region Snapshot Tests

    [Fact]
    public async Task GameTurn_Snapshot()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012");

        // Act & Assert
        await Verify(gameTurn);
    }

    [Fact]
    public async Task GameTurn_WithSpecificValues_Snapshot()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012",
            createdAt: new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            timeSpentInSeconds: 2.5,
            fromIndex: 10,
            toIndex: 20);

        // Act & Assert
        await Verify(gameTurn);
    }

    [Fact]
    public async Task InsertAsync_Result_Snapshot()
    {
        // Arrange
        var gameTurn = CreateTestGameTurn("turn0", "game0");

        // Act
        var result = await _handler.InsertTurnAsync(gameTurn, CancellationToken.None);

        // Assert
        await Verify(result);
    }

    #endregion
}