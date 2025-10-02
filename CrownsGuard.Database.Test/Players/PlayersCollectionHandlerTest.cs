using CrownsGuard.Database.Database;
using CrownsGuard.Database.Players;
using FluentResults;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Database.Test.Players;

public class PlayersCollectionHandlerTest : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoCollection<RegisteredPlayer> _playersCollection;
    private readonly PlayersCollectionHandler _handler;

    public PlayersCollectionHandlerTest()
    {
        // Start MongoDB test instance
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Create test database and collection
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var database = client.GetDatabase("TestBattleChess");
        _playersCollection = database.GetCollection<RegisteredPlayer>("Players");

        // Create a mock database client for testing
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players).Returns(_playersCollection);
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        // Create a mock logger for testing
        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();

        // Create handler with mocked dependencies
        _handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Helper Methods

    private RegisteredPlayer CreateTestPlayer(
        string id,
        string name,
        string emailHash,
        string passwordHash = "hashedpassword",
        string passwordSalt = "salt",
        short elo = 1500,
        byte[]? unlockedFigures = null,
        int[]? map = null)
    {
        return new RegisteredPlayer
        {
            Id = id,
            Name = name,
            EmailHash = emailHash,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Elo = elo,
            UnlockedFigures = unlockedFigures ?? new byte[] { 1, 2, 3, 4, 5 },
            Map = map ?? new[] { 0, 1, 2, 3, 4 }
        };
    }

    private async Task<List<RegisteredPlayer>> GetAllPlayersAsync()
    {
        return await _playersCollection.Find(_ => true).ToListAsync();
    }

    private async Task InsertTestPlayerAsync(RegisteredPlayer player)
    {
        await _playersCollection.InsertOneAsync(player);
    }

    #endregion

    #region FindPlayerByIdAsync Tests

    [Fact]
    public async Task FindPlayerByIdAsync_ValidId_ReturnsPlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(player.Id, result.Value.Id);
        Assert.Equal(player.Name, result.Value.Name);
        Assert.Equal(player.Elo, result.Value.Elo);
        Assert.Equal(player.EmailHash, result.Value.EmailHash);
    }

    [Fact]
    public async Task FindPlayerByIdAsync_InvalidId_ReturnsFailure()
    {
        // Arrange
        var invalidId = "507f1f77bcf86cd799439099";

        // Act
        var result = await _handler.FindPlayerByIdAsync(invalidId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }

    [Fact]
    public async Task FindPlayerByIdAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439014";

        // Act
        var result = await _handler.FindPlayerByIdAsync(playerId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindPlayerByIdAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439015";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.FindPlayerByIdAsync(playerId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindPlayerByIdAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();
        var handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.FindPlayerByIdAsync("507f1f77bcf86cd799439016", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region FindPlayerByNameAsync Tests

    [Fact]
    public async Task FindPlayerByNameAsync_ValidName_ReturnsPlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.FindPlayerByNameAsync(player.Name, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(player.Id, result.Value.Id);
        Assert.Equal(player.Name, result.Value.Name);
        Assert.Equal(player.Elo, result.Value.Elo);
        Assert.Equal(player.EmailHash, result.Value.EmailHash);
    }

    [Fact]
    public async Task FindPlayerByNameAsync_InvalidName_ReturnsFailure()
    {
        // Arrange
        var playerName = "Non-existent Player";

        // Act
        var result = await _handler.FindPlayerByNameAsync(playerName, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }

    [Fact]
    public async Task FindPlayerByNameAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var playerName = "Test Player Timeout";

        // Act
        var result = await _handler.FindPlayerByNameAsync(playerName, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindPlayerByNameAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var playerName = "Test Player Cancelled";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.FindPlayerByNameAsync(playerName, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindPlayerByNameAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();
        var handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.FindPlayerByNameAsync("Test Player Exception", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region FindPlayerByEmailHashAsync Tests

    [Fact]
    public async Task FindPlayerByEmailHashAsync_ValidEmailHash_ReturnsPlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.FindPlayerByEmailHashAsync(player.EmailHash, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(player.Id, result.Value.Id);
        Assert.Equal(player.Name, result.Value.Name);
        Assert.Equal(player.Elo, result.Value.Elo);
        Assert.Equal(player.EmailHash, result.Value.EmailHash);
    }

    [Fact]
    public async Task FindPlayerByEmailHashAsync_InvalidEmailHash_ReturnsFailure()
    {
        // Arrange
        var invalidEmailHash = "invalid_email_hash";

        // Act
        var result = await _handler.FindPlayerByEmailHashAsync(invalidEmailHash, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }

    [Fact]
    public async Task FindPlayerByEmailHashAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var emailHash = "test_timeout@example.com_hash";

        // Act
        var result = await _handler.FindPlayerByEmailHashAsync(emailHash, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindPlayerByEmailHashAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var emailHash = "test_cancelled@example.com_hash";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.FindPlayerByEmailHashAsync(emailHash, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindPlayerByEmailHashAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();
        var handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.FindPlayerByEmailHashAsync("test_exception@example.com_hash", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region InsertPlayerAsync Tests

    [Fact]
    public async Task InsertPlayerAsync_ValidPlayer_InsertsSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);

        // Act
        var result = await _handler.InsertPlayerAsync(player, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedPlayers = await GetAllPlayersAsync();
        Assert.Single(insertedPlayers);
        Assert.Equal(player.Id, insertedPlayers.First().Id);
        Assert.Equal(player.Name, insertedPlayers.First().Name);
        Assert.Equal(player.Elo, insertedPlayers.First().Elo);
    }

    [Fact]
    public async Task InsertPlayerAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var player = CreateTestPlayer("507f1f77bcf86cd799439011", "Test Player Timeout", "test_timeout@example.com_hash");

        // Act
        var result = await _handler.InsertPlayerAsync(player, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertPlayerAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var player = CreateTestPlayer("507f1f77bcf86cd799439011", "Test Player Cancelled", "test_cancelled@example.com_hash");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.InsertPlayerAsync(player, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertPlayerAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();
        var handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        var player = CreateTestPlayer("507f1f77bcf86cd799439011", "Test Player Exception", "test_exception@example.com_hash");

        // Act
        var result = await handler.InsertPlayerAsync(player, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region UpdatePlayerEloAsync Tests

    [Fact]
    public async Task UpdatePlayerEloAsync_ValidPlayer_UpdatesEloSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);
        var newElo = 1600;

        // Act
        var result = await _handler.UpdatePlayerEloAsync(player.Id, newElo, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Equal(newElo, updatedPlayer.Elo);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_InvalidPlayerId_ReturnsFailure()
    {
        // Arrange
        var invalidPlayerId = "507f1f77bcf86cd799439099";
        var newElo = 1600;

        // Act
        var result = await _handler.UpdatePlayerEloAsync(invalidPlayerId, newElo, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to update player elo", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var newElo = 1600;

        // Act
        var result = await _handler.UpdatePlayerEloAsync(playerId, newElo, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var newElo = 1600;
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.UpdatePlayerEloAsync(playerId, newElo, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();
        var handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.UpdatePlayerEloAsync("507f1f77bcf86cd799439011", 1600, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region UpdatePlayerSetupAsync Tests

    [Fact]
    public async Task UpdatePlayerSetupAsync_ValidPlayer_UpdatesSetupSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            map: new[] { 0, 1, 2, 3, 4 });
        await InsertTestPlayerAsync(player);
        var newMap = new[] { 5, 6, 7, 8, 9 };

        // Act
        var result = await _handler.UpdatePlayerSetupAsync(player.Id, newMap, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Equal(newMap, updatedPlayer.Map);
    }

    [Fact]
    public async Task UpdatePlayerSetupAsync_InvalidPlayerId_ReturnsFailure()
    {
        // Arrange
        var invalidPlayerId = "507f1f77bcf86cd799439099";
        var newMap = new[] { 5, 6, 7, 8, 9 };

        // Act
        var result = await _handler.UpdatePlayerSetupAsync(invalidPlayerId, newMap, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to update player setup", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdatePlayerSetupAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var newMap = new[] { 5, 6, 7, 8, 9 };

        // Act
        var result = await _handler.UpdatePlayerSetupAsync(playerId, newMap, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdatePlayerSetupAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var newMap = new[] { 5, 6, 7, 8, 9 };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.UpdatePlayerSetupAsync(playerId, newMap, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region UpdatePlayerUnlockedFiguresAsync Tests

    [Fact]
    public async Task UpdatePlayerUnlockedFiguresAsync_ValidPlayer_UpdatesUnlockedFiguresSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            unlockedFigures: new byte[] { 1, 2, 3, 4, 5 });
        await InsertTestPlayerAsync(player);
        var newUnlockedFigures = new byte[] { 6, 7, 8, 9, 10 };

        // Act
        var result = await _handler.UpdatePlayerUnlockedFiguresAsync(player.Id, newUnlockedFigures, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Equal(newUnlockedFigures, updatedPlayer.UnlockedFigures);
    }

    [Fact]
    public async Task UpdatePlayerUnlockedFiguresAsync_InvalidPlayerId_ReturnsFailure()
    {
        // Arrange
        var invalidPlayerId = "507f1f77bcf86cd799439099";
        var newUnlockedFigures = new byte[] { 6, 7, 8, 9, 10 };

        // Act
        var result = await _handler.UpdatePlayerUnlockedFiguresAsync(invalidPlayerId, newUnlockedFigures, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to update unlocked figures", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdatePlayerUnlockedFiguresAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var newUnlockedFigures = new byte[] { 6, 7, 8, 9, 10 };

        // Act
        var result = await _handler.UpdatePlayerUnlockedFiguresAsync(playerId, newUnlockedFigures, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdatePlayerUnlockedFiguresAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var newUnlockedFigures = new byte[] { 6, 7, 8, 9, 10 };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.UpdatePlayerUnlockedFiguresAsync(playerId, newUnlockedFigures, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region WaitForPlayerEloUpdateAsync Tests

    [Fact]
    public async Task WaitForPlayerEloUpdateAsync_PlayerEloUpdated_ReturnsPlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var waitTask = _handler.WaitForPlayerEloUpdateAsync(player.Id, player.Elo, CancellationToken.None, 30);
        
        Thread.Sleep(200);

        // Update the player's ELO while waiting
        await _handler.UpdatePlayerEloAsync(player.Id, 1600, CancellationToken.None, 30);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(player.Id, result.Value.Id);
        Assert.Equal(1600, result.Value.Elo); // Should have the updated ELO
    }

    [Fact]
    public async Task WaitForPlayerEloUpdateAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439017";

        // Act
        var result = await _handler.WaitForPlayerEloUpdateAsync(playerId, 100, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForPlayerEloUpdateAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439018";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.WaitForPlayerEloUpdateAsync(playerId, 100, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForPlayerEloUpdateAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.Players)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<PlayersCollectionHandler>>();
        var handler = new PlayersCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.WaitForPlayerEloUpdateAsync("507f1f77bcf86cd799439019", 100, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region Edge Cases and Data Validation Tests

    [Fact]
    public async Task FindPlayerByIdAsync_EmptyId_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindPlayerByIdAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindPlayerByIdAsync_InvalidObjectIdFormat_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindPlayerByIdAsync("invalid-object-id-format", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindPlayerByNameAsync_EmptyName_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindPlayerByNameAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindPlayerByNameAsync_WhitespaceName_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindPlayerByNameAsync("   ", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindPlayerByEmailHashAsync_EmptyEmailHash_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindPlayerByEmailHashAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindPlayerByEmailHashAsync_WhitespaceEmailHash_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindPlayerByEmailHashAsync("   ", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_ZeroElo_UpdatesSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.UpdatePlayerEloAsync(player.Id, 0, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Equal(0, updatedPlayer.Elo);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_NegativeElo_UpdatesSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.UpdatePlayerEloAsync(player.Id, -100, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Equal(-100, updatedPlayer.Elo);
    }

    [Fact]
    public async Task UpdatePlayerSetupAsync_EmptyMap_UpdatesSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            map: new[] { 0, 1, 2, 3, 4 });
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.UpdatePlayerSetupAsync(player.Id, Array.Empty<int>(), CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Empty(updatedPlayer.Map);
    }

    [Fact]
    public async Task UpdatePlayerUnlockedFiguresAsync_EmptyUnlockedFigures_UpdatesSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            unlockedFigures: new byte[] { 1, 2, 3, 4, 5 });
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.UpdatePlayerUnlockedFiguresAsync(player.Id, Array.Empty<byte>(), CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Empty(updatedPlayer.UnlockedFigures);
    }

    #endregion

    #region Business Logic Edge Cases Tests

    [Fact]
    public async Task InsertPlayerAsync_DuplicatePlayerId_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var player1 = CreateTestPlayer(playerId, "Test Player 1", "test1@example.com_hash");
        var player2 = CreateTestPlayer(playerId, "Test Player 2", "test2@example.com_hash"); // Same ID

        // Act
        await _handler.InsertPlayerAsync(player1, CancellationToken.None, 30);
        var result = await _handler.InsertPlayerAsync(player2, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task UpdatePlayerEloAsync_SameElo_NoChangeMade()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act
        var result = await _handler.UpdatePlayerEloAsync(player.Id, 1500, CancellationToken.None, 30); // Same ELO

        // Assert
        Assert.True(result.IsSuccess);
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.Equal(1500, updatedPlayer.Elo);
    }

    #endregion

    #region Advanced Concurrency and Race Condition Tests

    [Fact]
    public async Task Concurrent_MultipleFindByIdAsync_SamePlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        // Act - Start multiple concurrent read operations
        var tasks = new List<Task<Result<RegisteredPlayer>>>
        {
            _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30),
            _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30),
            _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30),
            _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30),
            _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30)
        };

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var result = await task;
            Assert.True(result.IsSuccess);
            Assert.Equal(player.Id, result.Value.Id);
            Assert.Equal(player.Name, result.Value.Name);
            Assert.Equal(player.Elo, result.Value.Elo);
        }
    }

    [Fact]
    public async Task Concurrent_ReadAndWriteOperations_SamePlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash");
        await InsertTestPlayerAsync(player);

        var newElo = 1600;
        var newMap = new[] { 5, 6, 7, 8, 9 };
        var newUnlockedFigures = new byte[] { 6, 7, 8, 9, 10 };

        // Act - Mix of read and write operations
        var task1 = _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30);
        var task2 = _handler.FindPlayerByNameAsync(player.Name, CancellationToken.None, 30);
        var task3 = _handler.UpdatePlayerEloAsync(player.Id, newElo, CancellationToken.None, 30);
        var task4 = _handler.UpdatePlayerSetupAsync(player.Id, newMap, CancellationToken.None, 30);
        var task5 = _handler.UpdatePlayerUnlockedFiguresAsync(player.Id, newUnlockedFigures, CancellationToken.None, 30);
        var task6 = _handler.FindPlayerByIdAsync(player.Id, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2, task3, task4, task5, task6);
        var result1 = await task1;
        var result2 = await task2;
        var result3 = await task3;
        var result4 = await task4;
        var result5 = await task5;
        var result6 = await task6;

        // Assert
        // All operations should complete successfully
        Assert.True(result1.IsSuccess); // FindById
        Assert.True(result2.IsSuccess); // FindByName
        Assert.True(result3.IsSuccess); // UpdatePlayerElo
        Assert.True(result4.IsSuccess); // UpdatePlayerSetup
        Assert.True(result5.IsSuccess); // UpdatePlayerUnlockedFigures
        Assert.True(result6.IsSuccess); // FindById after updates

        // Verify the player was updated
        var updatedPlayer = result6.Value;
        Assert.Equal(newElo, updatedPlayer.Elo);
        Assert.Equal(newMap, updatedPlayer.Map);
        Assert.Equal(newUnlockedFigures, updatedPlayer.UnlockedFigures);
    }

    [Fact]
    public async Task Concurrent_UpdatePlayerEloAsync_SamePlayer_DifferentElos()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);
        await InsertTestPlayerAsync(player);

        var elo1 = 1600;
        var elo2 = 1700;
        var elo3 = 1800;

        // Act - Start multiple concurrent update operations
        var task1 = _handler.UpdatePlayerEloAsync(player.Id, elo1, CancellationToken.None, 30);
        var task2 = _handler.UpdatePlayerEloAsync(player.Id, elo2, CancellationToken.None, 30);
        var task3 = _handler.UpdatePlayerEloAsync(player.Id, elo3, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2, task3);

        // Assert
        // All operations should complete successfully (last write wins)
        Assert.True((await task1).IsSuccess);
        Assert.True((await task2).IsSuccess);
        Assert.True((await task3).IsSuccess);

        // Verify the player has one of the updated ELO values
        var updatedPlayers = await GetAllPlayersAsync();
        var updatedPlayer = updatedPlayers.FirstOrDefault(p => p.Id == player.Id);
        Assert.NotNull(updatedPlayer);
        Assert.True(updatedPlayer.Elo == elo1 || updatedPlayer.Elo == elo2 || updatedPlayer.Elo == elo3);
    }

    [Fact]
    public async Task RaceCondition_InsertAndImmediateUpdate_SamePlayer()
    {
        // Arrange
        var player = CreateTestPlayer(
            id: "507f1f77bcf86cd799439011",
            name: "TestPlayer",
            emailHash: "test@example.com_hash",
            elo: 1500);

        // Act - Try to insert and immediately update
        var insertTask = _handler.InsertPlayerAsync(player, CancellationToken.None, 30);
        var updateTask = _handler.UpdatePlayerEloAsync(player.Id, 1600, CancellationToken.None, 30);

        var results = await Task.WhenAll(insertTask, updateTask);

        // Assert
        // At least one operation should succeed
        var successCount = results.Count(r => r.IsSuccess);
        Assert.True(successCount >= 1);

        // Verify final state
        var allPlayers = await GetAllPlayersAsync();
        var playerExists = allPlayers.Any(p => p.Id == player.Id);

        // Either the player exists (insert succeeded) or doesn't exist (update failed as expected)
        if (playerExists)
        {
            var existingPlayer = allPlayers.First(p => p.Id == player.Id);
            // If insert succeeded, ELO might be original or updated depending on timing
            Assert.True(existingPlayer.Elo == 1500 || existingPlayer.Elo == 1600);
        }
    }

    #endregion
}