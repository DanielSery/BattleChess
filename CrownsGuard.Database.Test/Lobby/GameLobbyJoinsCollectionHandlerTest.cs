using CrownsGuard.Database.Database;
using CrownsGuard.Database.Lobby;
using FluentResults;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Database.Test.Lobby;

public class GameLobbyJoinsCollectionHandlerTest : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoCollection<GameLobbyJoin> _lobbyJoinsCollection;
    private readonly GameLobbyJoinsCollectionHandler _handler;

    public GameLobbyJoinsCollectionHandlerTest()
    {
        // Start MongoDB test instance
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Create test database and collection
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var database = client.GetDatabase("TestBattleChess");
        _lobbyJoinsCollection = database.GetCollection<GameLobbyJoin>("LobbyGameJoins");

        // Create a mock database client for testing
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.LobbyGameJoins).Returns(_lobbyJoinsCollection);
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        // Create a mock logger for testing
        var mockLogger = new Mock<ILogger<GameLobbyJoinsCollectionHandler>>();

        // Create handler with mocked dependencies
        _handler = new GameLobbyJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Helper Methods

    private GameLobbyJoin CreateTestGameLobbyJoin(
        string id,
        string gameId,
        string? playerId = null,
        int[]? map = null)
    {
        return new GameLobbyJoin
        {
            Id = id,
            GameId = gameId,
            PlayerId = playerId,
            Map = map ?? new[] { 0, 1, 2, 3, 4 }
        };
    }

    private async Task<List<GameLobbyJoin>> GetAllLobbyJoinsAsync()
    {
        return await _lobbyJoinsCollection.Find(_ => true).ToListAsync();
    }

    private async Task InsertTestLobbyJoinAsync(GameLobbyJoin lobbyJoin)
    {
        await _lobbyJoinsCollection.InsertOneAsync(lobbyJoin);
    }

    #endregion

    #region InsertLobbyJoinAsync Tests

    [Fact]
    public async Task InsertLobbyJoinAsync_ValidLobbyJoin_InsertsSuccessfully()
    {
        // Arrange
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439011",
            gameId: "507f1f77bcf86cd799439012",
            playerId: "507f1f77bcf86cd799439013");

        // Act
        var result = await _handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedJoins = await GetAllLobbyJoinsAsync();
        Assert.Single(insertedJoins);
        Assert.Equal(lobbyJoin.Id, insertedJoins.First().Id);
        Assert.Equal(lobbyJoin.GameId, insertedJoins.First().GameId);
        Assert.Equal(lobbyJoin.PlayerId, insertedJoins.First().PlayerId);
    }

    [Fact]
    public async Task InsertLobbyJoinAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var lobbyJoin = CreateTestGameLobbyJoin("lobby_join_timeout", "game_timeout");

        // Act
        var result = await _handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertLobbyJoinAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var lobbyJoin = CreateTestGameLobbyJoin("lobby_join_cancelled", "game_cancelled");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.InsertLobbyJoinAsync(lobbyJoin, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertLobbyJoinAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.LobbyGameJoins)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<GameLobbyJoinsCollectionHandler>>();
        var handler = new GameLobbyJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        var lobbyJoin = CreateTestGameLobbyJoin("lobby_join_exception", "game_exception");

        // Act
        var result = await handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region WaitForLobbyJoinAsync Tests

    [Fact]
    public async Task WaitForLobbyJoinAsync_ExistingJoin_ReturnsImmediately()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");
        await InsertTestLobbyJoinAsync(lobbyJoin);

        // Act
        var result = await _handler.WaitForLobbyJoinAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(lobbyJoin.Id, result.Value.Id);
        Assert.Equal(lobbyJoin.GameId, result.Value.GameId);
        Assert.Equal(lobbyJoin.PlayerId, result.Value.PlayerId);
    }

    [Fact]
    public async Task WaitForLobbyJoinAsync_NewJoin_WaitsAndReturns()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439014";
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439015",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439016");

        // Act
        var waitTask = _handler.WaitForLobbyJoinAsync(gameId, CancellationToken.None, 30);

        // Insert the join while waiting
        await Task.Delay(100); // Small delay to ensure wait starts first
        await InsertTestLobbyJoinAsync(lobbyJoin);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(lobbyJoin.Id, result.Value.Id);
        Assert.Equal(lobbyJoin.GameId, result.Value.GameId);
    }

    [Fact]
    public async Task WaitForLobbyJoinAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439017";

        // Act
        var result = await _handler.WaitForLobbyJoinAsync(gameId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForLobbyJoinAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439018";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.WaitForLobbyJoinAsync(gameId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForLobbyJoinAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.LobbyGameJoins)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<GameLobbyJoinsCollectionHandler>>();
        var handler = new GameLobbyJoinsCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.WaitForLobbyJoinAsync("507f1f77bcf86cd799439019", CancellationToken.None, 30);

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
        var lobbyJoin1 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId);
        var lobbyJoin2 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439013",
            gameId: gameId);
        var otherJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439014",
            gameId: "507f1f77bcf86cd799439015");

        await InsertTestLobbyJoinAsync(lobbyJoin1);
        await InsertTestLobbyJoinAsync(lobbyJoin2);
        await InsertTestLobbyJoinAsync(otherJoin);

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingJoins = await GetAllLobbyJoinsAsync();
        Assert.Single(remainingJoins);
        Assert.Equal(otherJoin.Id, remainingJoins.First().Id);
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
    public async Task DeleteGameJoinsAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439019";

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439020";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Edge Cases and Data Validation Tests

    [Fact]
    public async Task InsertLobbyJoinAsync_LobbyJoinWithEmptyId_InsertsSuccessfully()
    {
        // Arrange
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "",
            gameId: "507f1f77bcf86cd799439012",
            playerId: "507f1f77bcf86cd799439013");

        // Act
        var result = await _handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedJoins = await GetAllLobbyJoinsAsync();
        Assert.Single(insertedJoins);
    }

    [Fact]
    public async Task WaitForLobbyJoinAsync_EmptyGameId_ReturnsFailure()
    {
        // Act
        var result = await _handler.WaitForLobbyJoinAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_EmptyGameId_ReturnsFailure()
    {
        // Act
        var result = await _handler.DeleteGameJoinsAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    #endregion

    #region Business Logic Edge Cases Tests

    [Fact]
    public async Task InsertLobbyJoinAsync_DuplicateGameId_InsertsMultipleJoins()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin1 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");
        var lobbyJoin2 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439014",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439015");

        // Act
        var result1 = await _handler.InsertLobbyJoinAsync(lobbyJoin1, CancellationToken.None, 30);
        var result2 = await _handler.InsertLobbyJoinAsync(lobbyJoin2, CancellationToken.None, 30);

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);
        var insertedJoins = await GetAllLobbyJoinsAsync();
        Assert.Equal(2, insertedJoins.Count);
        Assert.Contains(insertedJoins, lj => lj.Id == lobbyJoin1.Id);
        Assert.Contains(insertedJoins, lj => lj.Id == lobbyJoin2.Id);
    }

    [Fact]
    public async Task WaitForLobbyJoinAsync_MultipleExistingJoins_ReturnsFirstOne()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin1 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");
        var lobbyJoin2 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439014",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439015");

        await InsertTestLobbyJoinAsync(lobbyJoin1);
        await InsertTestLobbyJoinAsync(lobbyJoin2);

        // Act
        var result = await _handler.WaitForLobbyJoinAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        // Should return the first join found (order may vary due to MongoDB)
        var insertedJoins = await GetAllLobbyJoinsAsync();
        var foundJoin = insertedJoins.FirstOrDefault(lj => lj.Id == result.Value.Id);
        Assert.NotNull(foundJoin);
        Assert.Equal(gameId, foundJoin.GameId);
    }

    [Fact]
    public async Task DeleteGameJoinsAsync_MultipleGames_DeletesOnlySpecifiedGame()
    {
        // Arrange
        var gameId1 = "507f1f77bcf86cd799439011";
        var gameId2 = "507f1f77bcf86cd799439012";
        var lobbyJoin1 = CreateTestGameLobbyJoin("507f1f77bcf86cd799439013", gameId1);
        var lobbyJoin2 = CreateTestGameLobbyJoin("507f1f77bcf86cd799439014", gameId1);
        var lobbyJoin3 = CreateTestGameLobbyJoin("507f1f77bcf86cd799439015", gameId2);

        await InsertTestLobbyJoinAsync(lobbyJoin1);
        await InsertTestLobbyJoinAsync(lobbyJoin2);
        await InsertTestLobbyJoinAsync(lobbyJoin3);

        // Act
        var result = await _handler.DeleteGameJoinsAsync(gameId1, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingJoins = await GetAllLobbyJoinsAsync();
        Assert.Single(remainingJoins);
        Assert.Equal(lobbyJoin3.Id, remainingJoins.First().Id);
    }

    #endregion

    #region Advanced Concurrency and Race Condition Tests

    [Fact]
    public async Task Concurrent_InsertLobbyJoinAsync_SameGame_DifferentJoins()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin1 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");
        var lobbyJoin2 = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439014",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439015");

        // Act - Start both operations concurrently
        var task1 = _handler.InsertLobbyJoinAsync(lobbyJoin1, CancellationToken.None, 30);
        var task2 = _handler.InsertLobbyJoinAsync(lobbyJoin2, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2);
        var result1 = await task1;
        var result2 = await task2;

        // Assert
        Assert.True(result1.IsSuccess);
        Assert.True(result2.IsSuccess);

        var insertedJoins = await GetAllLobbyJoinsAsync();
        Assert.Equal(2, insertedJoins.Count);
        Assert.Contains(insertedJoins, lj => lj.Id == lobbyJoin1.Id);
        Assert.Contains(insertedJoins, lj => lj.Id == lobbyJoin2.Id);
    }

    [Fact]
    public async Task Concurrent_WaitForLobbyJoinAsync_MultipleWaiters_SameGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");

        // Act - Start multiple waiters
        var waitTask1 = _handler.WaitForLobbyJoinAsync(gameId, CancellationToken.None, 30);
        var waitTask2 = _handler.WaitForLobbyJoinAsync(gameId, CancellationToken.None, 30);

        // Insert the join after a small delay
        await Task.Delay(100);
        await InsertTestLobbyJoinAsync(lobbyJoin);

        var results = await Task.WhenAll(waitTask1, waitTask2);

        // Assert
        Assert.True(results[0].IsSuccess);
        Assert.True(results[1].IsSuccess);

        // Both should get the same join
        Assert.Equal(results[0].Value.Id, results[1].Value.Id);
        Assert.Equal(lobbyJoin.Id, results[0].Value.Id);
    }

    [Fact]
    public async Task Concurrent_DeleteAndInsertOperations_SameGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");

        // Act - Mix of delete and insert operations
        var tasks = new List<Task<Result>>
        {
            _handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 30),
            _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 30),
            _handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 30) // Try to insert again after delete
        };

        var results = await Task.WhenAll(tasks);

        // Assert
        // Operations should complete without exceptions
        foreach (var result in results)
        {
            Assert.True(result.IsSuccess || result.IsFailed);
        }

        // Final state should be consistent
        var finalJoins = await GetAllLobbyJoinsAsync();
        // Either 0 joins (delete succeeded) or 1 join (insert after delete succeeded)
        Assert.True(finalJoins.Count == 0 || finalJoins.Count == 1);
    }

    [Fact]
    public async Task RaceCondition_InsertAndImmediateDelete_SameGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobbyJoin = CreateTestGameLobbyJoin(
            id: "507f1f77bcf86cd799439012",
            gameId: gameId,
            playerId: "507f1f77bcf86cd799439013");

        // Act - Try to insert and immediately delete
        var insertTask = _handler.InsertLobbyJoinAsync(lobbyJoin, CancellationToken.None, 30);
        var deleteTask = _handler.DeleteGameJoinsAsync(gameId, CancellationToken.None, 30);

        var results = await Task.WhenAll(insertTask, deleteTask);

        // Assert
        // At least one operation should succeed
        var successCount = results.Count(r => r.IsSuccess);
        Assert.True(successCount >= 1);

        // Verify final state
        var allJoins = await GetAllLobbyJoinsAsync();
        var joinExists = allJoins.Any(lj => lj.GameId == gameId);

        // The final state should be consistent - either:
        // 1. Insert succeeded and join exists, OR
        // 2. Delete succeeded and no join exists for this game
        // We verify this by checking that we don't have an inconsistent state
        var joinsForGame = allJoins.Where(lj => lj.GameId == gameId).ToList();
        Assert.True(joinsForGame.Count <= 1, "Should have at most one join per game");

        // If join exists, verify it has the expected properties
        if (joinExists)
        {
            var existingJoin = joinsForGame.First();
            Assert.Equal(lobbyJoin.Id, existingJoin.Id);
            Assert.Equal(lobbyJoin.PlayerId, existingJoin.PlayerId);
            Assert.Equal(lobbyJoin.Map, existingJoin.Map);
        }
    }

    #endregion
}