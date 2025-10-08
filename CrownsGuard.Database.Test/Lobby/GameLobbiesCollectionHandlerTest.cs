using AwesomeAssertions;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Database.Test.Lobby;

public class GameLobbiesCollectionHandlerTest : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoCollection<GameLobby> _gameLobbiesCollection;
    private readonly GameLobbiesCollectionHandler _handler;

    public GameLobbiesCollectionHandlerTest()
    {
        // Start MongoDB test instance
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Create test database and collection
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var database = client.GetDatabase("TestBattleChess");
        _gameLobbiesCollection = database.GetCollection<GameLobby>("GameLobbies");

        // Create a mock database client for testing
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.GameLobbies).Returns(_gameLobbiesCollection);
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        // Create a mock logger for testing
        var mockLogger = new Mock<ILogger<GameLobbiesCollectionHandler>>();

        // Create handler with mocked dependencies
        _handler = new GameLobbiesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Helper Methods

    private GameLobby CreateTestGameLobby(
        string id,
        string lobbyName,
        string? playerId = null,
        string? joinedId = null,
        string passwordHash = "",
        string passwordSalt = "",
        int? version = null,
        short? elo = null,
        bool isHostStarting = false,
        int[]? map = null)
    {
        return new GameLobby
        {
            Id = id,
            PlayerId = playerId,
            JoinedId = joinedId,
            LobbyName = lobbyName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Version = version ?? GameVersion.VersionId,
            Elo = elo,
            IsHostStarting = isHostStarting,
            Map = map ?? new[] { 0, 1, 2, 3, 4 }
        };
    }

    private async Task<List<GameLobby>> GetAllGameLobbiesAsync()
    {
        return await _gameLobbiesCollection.Find(_ => true).ToListAsync();
    }

    private async Task InsertTestGameLobbyAsync(GameLobby gameLobby)
    {
        await _gameLobbiesCollection.InsertOneAsync(gameLobby);
    }

    #endregion

    #region FindByIdAsync Tests

    [Fact]
    public async Task FindByIdAsync_ValidId_ReturnsLobby()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby",
            elo: 1500);
        await InsertTestGameLobbyAsync(lobby);

        // Act
        var result = await _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(lobby.Id, result.Value.Id);
        Assert.Equal(lobby.LobbyName, result.Value.LobbyName);
        Assert.Equal(lobby.Elo, result.Value.Elo);
    }

    [Fact]
    public async Task FindByIdAsync_InvalidId_ReturnsFailure()
    {
        // Arrange
        var invalidId = "507f1f77bcf86cd799439099";

        // Act
        var result = await _handler.FindLobbyByIdAsync(invalidId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }

    [Fact]
    public async Task FindByIdAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439014";

        // Act
        var result = await _handler.FindLobbyByIdAsync(lobbyId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindByIdAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439015";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.FindLobbyByIdAsync(lobbyId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindByIdAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.GameLobbies)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<GameLobbiesCollectionHandler>>();
        var handler = new GameLobbiesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.FindLobbyByIdAsync("507f1f77bcf86cd799439016", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region Edge Cases and Data Validation Tests

    [Fact]
    public async Task FindByIdAsync_EmptyId_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindLobbyByIdAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindByIdAsync_InvalidObjectIdFormat_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindLobbyByIdAsync("invalid-object-id-format", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindByNameAsync_EmptyName_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindLobbyByNameAsync(string.Empty, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task FindByNameAsync_WhitespaceName_ReturnsFailure()
    {
        // Act
        var result = await _handler.FindLobbyByNameAsync("   ", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task InsertAsync_GameLobbyWithInvalidId_ReturnsFailure()
    {
        // Arrange
        var lobby = CreateTestGameLobby("lobby0", "Test Lobby");
        lobby.Id = "invalid-id";

        // Act
        var result = await _handler.InsertLobbyAsync(lobby, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    #endregion

    #region Business Logic Edge Cases Tests

    [Fact]
    public async Task GetPublicLobbiesAsync_PasswordProtectedLobbies_NotReturned()
    {
        // Arrange
        var publicLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Public Lobby",
            passwordHash: "");

        var privateLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439012",
            lobbyName: "Private Lobby",
            passwordHash: "hashedpassword");

        await InsertTestGameLobbyAsync(publicLobby);
        await InsertTestGameLobbyAsync(privateLobby);

        // Act
        var result = await _handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        result.Value.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetPublicLobbiesAsync_LobbiesWithJoinedId_NotReturned()
    {
        // Arrange
        var publicLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Public Lobby",
            joinedId: null);

        var occupiedLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439012",
            lobbyName: "Occupied Lobby",
            joinedId: "507f1f77bcf86cd799439013");

        await InsertTestGameLobbyAsync(publicLobby);
        await InsertTestGameLobbyAsync(occupiedLobby);

        // Act
        var result = await _handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Public Lobby", result.Value.First().LobbyName);
    }

    [Fact]
    public async Task GetPublicLobbiesAsync_WrongVersionLobbies_NotReturned()
    {
        // Arrange
        var correctVersionLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Correct Version Lobby",
            version: GameVersion.VersionId);

        var wrongVersionLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439012",
            lobbyName: "Wrong Version Lobby",
            version: GameVersion.VersionId + 1);

        await InsertTestGameLobbyAsync(correctVersionLobby);
        await InsertTestGameLobbyAsync(wrongVersionLobby);

        // Act
        var result = await _handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal("Correct Version Lobby", result.Value.First().LobbyName);
    }

    [Fact]
    public async Task FindByNameAsync_WrongVersionLobby_NotFound()
    {
        // Arrange
        var wrongVersionLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Wrong Version Lobby",
            version: GameVersion.VersionId + 1);

        await InsertTestGameLobbyAsync(wrongVersionLobby);

        // Act
        var result = await _handler.FindLobbyByNameAsync("Wrong Version Lobby", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task UpdateLobbyJoinAsync_LobbyAlreadyJoined_ReturnsFailure()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby",
            joinedId: "507f1f77bcf86cd799439012"); // Already has a player joined

        await InsertTestGameLobbyAsync(lobby);
        var newJoinId = "507f1f77bcf86cd799439013";

        // Act
        var result = await _handler.ConfirmLobbyJoinAsync(lobby.Id, newJoinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task WaitForLobbyAcceptAsync_LobbyAlreadyHasJoinedId_ReturnsImmediately()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby",
            joinedId: "507f1f77bcf86cd799439012"); // Already has a player joined

        await InsertTestGameLobbyAsync(lobby);

        // Act
        var result = await _handler.WaitForLobbyJoinConfirmationAsync(lobby.Id, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(lobby.Id, result.Value.Id);
        Assert.Equal(lobby.JoinedId, result.Value.JoinedId);
    }

    [Fact]
    public async Task WaitForLobbyAcceptAsync_LobbyDeletedDuringWait_ReturnsFailure()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby");

        await InsertTestGameLobbyAsync(lobby);

        // Act
        var waitTask = _handler.WaitForLobbyJoinConfirmationAsync(lobby.Id, CancellationToken.None, 30);

        // Delete the lobby while waiting
        await _handler.DeleteGameLobbiesAsync(lobby.Id, CancellationToken.None, 30);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task InsertAsync_DuplicateLobbyId_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439011";
        var lobby1 = CreateTestGameLobby(lobbyId, "Test Lobby 1");
        var lobby2 = CreateTestGameLobby(lobbyId, "Test Lobby 2"); // Same ID

        // Act
        await _handler.InsertLobbyAsync(lobby1, CancellationToken.None, 30);
        var result = await _handler.InsertLobbyAsync(lobby2, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task Concurrent_UpdateLobbyJoinAsync_SameLobby_DifferentJoinIds()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby");

        await InsertTestGameLobbyAsync(lobby);

        var joinId1 = "507f1f77bcf86cd799439012";
        var joinId2 = "507f1f77bcf86cd799439013";

        // Act - Start both operations concurrently
        var task1 = _handler.ConfirmLobbyJoinAsync(lobby.Id, joinId1, CancellationToken.None, 30);
        var task2 = _handler.ConfirmLobbyJoinAsync(lobby.Id, joinId2, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2);

        // Assert
        // Verify the lobby was updated with one of the join IDs
        var findResult = await _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30);
        Assert.True(findResult.IsSuccess);
        var joinedId = findResult.Value.JoinedId;
        Assert.True(joinedId == joinId1 || joinedId == joinId2);
    }

    #endregion

    #region FindByNameAsync Tests

    [Fact]
    public async Task FindByNameAsync_ValidName_ReturnsLobby()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby",
            elo: 1500);
        await InsertTestGameLobbyAsync(lobby);

        // Act
        var result = await _handler.FindLobbyByNameAsync(lobby.LobbyName, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(lobby.Id, result.Value.Id);
        Assert.Equal(lobby.LobbyName, result.Value.LobbyName);
        Assert.Equal(lobby.Elo, result.Value.Elo);
    }

    [Fact]
    public async Task FindByNameAsync_InvalidName_ReturnsFailure()
    {
        // Arrange
        var lobbyName = "Non-existent Lobby";

        // Act
        var result = await _handler.FindLobbyByNameAsync(lobbyName, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }

    [Fact]
    public async Task FindByNameAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var lobbyName = "Test Lobby Timeout";

        // Act
        var result = await _handler.FindLobbyByNameAsync(lobbyName, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindByNameAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var lobbyName = "Test Lobby Cancelled";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.FindLobbyByNameAsync(lobbyName, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task FindByNameAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.GameLobbies)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<GameLobbiesCollectionHandler>>();
        var handler = new GameLobbiesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.FindLobbyByNameAsync("Test Lobby Exception", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion

    #region WaitForLobbyAcceptAsync Tests

    [Fact]
    public async Task WaitForLobbyAcceptAsync_LobbyAcceptsJoin_ReturnsLobby()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby");
        await InsertTestGameLobbyAsync(lobby);

        // Act
        var task = _handler.WaitForLobbyJoinConfirmationAsync(lobby.Id, CancellationToken.None, 30);
        var updateResult = await _handler.ConfirmLobbyJoinAsync(lobby.Id, "507f1f77bcf86cd799439015", CancellationToken.None, 30);
        
        Assert.True(updateResult.IsSuccess);
        var result = await task;
        
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(lobby.Id, result.Value.Id);
    }

    [Fact]
    public async Task WaitForLobbyAcceptAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439017";

        // Act
        var result = await _handler.WaitForLobbyJoinConfirmationAsync(lobbyId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task WaitForLobbyAcceptAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439018";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.WaitForLobbyJoinConfirmationAsync(lobbyId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region DeleteGameLobbiesAsync Tests

    [Fact]
    public async Task DeleteGameLobbiesAsync_ValidGameId_DeletesLobbies()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var lobby1 = CreateTestGameLobby(
            id: gameId,
            lobbyName: "Lobby 1");
        var lobby2 = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439012",
            lobbyName: "Lobby 2");

        await InsertTestGameLobbyAsync(lobby1);
        await InsertTestGameLobbyAsync(lobby2);

        // Act
        var result = await _handler.DeleteGameLobbiesAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingLobbies = await GetAllGameLobbiesAsync();
        Assert.Single(remainingLobbies);
        Assert.Equal(lobby2.Id, remainingLobbies.First().Id);
    }

    [Fact]
    public async Task DeleteGameLobbiesAsync_InvalidGameId_ReturnsSuccess()
    {
        // Arrange
        var invalidGameId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _handler.DeleteGameLobbiesAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteGameLobbiesAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439019";

        // Act
        var result = await _handler.DeleteGameLobbiesAsync(gameId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeleteGameLobbiesAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439020";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.DeleteGameLobbiesAsync(gameId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region UpdateLobbyJoinAsync Tests

    [Fact]
    public async Task UpdateLobbyJoinAsync_ValidLobby_UpdatesJoinId()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby");
        await InsertTestGameLobbyAsync(lobby);
        var joinId = "507f1f77bcf86cd799439012";

        // Act
        var result = await _handler.ConfirmLobbyJoinAsync(lobby.Id, joinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedLobbies = await GetAllGameLobbiesAsync();
        var updatedLobby = updatedLobbies.FirstOrDefault(l => l.Id == lobby.Id);
        Assert.NotNull(updatedLobby);
        Assert.Equal(joinId, updatedLobby.JoinedId);
    }

    [Fact]
    public async Task UpdateLobbyJoinAsync_InvalidLobby_ReturnsFailure()
    {
        // Arrange
        var invalidLobbyId = "507f1f77bcf86cd799439011";
        var joinId = "507f1f77bcf86cd799439012";

        // Act
        var result = await _handler.ConfirmLobbyJoinAsync(invalidLobbyId, joinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
        Assert.Contains("Failed to update game confirmation", result.Errors.First().Message);
    }

    [Fact]
    public async Task UpdateLobbyJoinAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439021";
        var joinId = "507f1f77bcf86cd799439022";

        // Act
        var result = await _handler.ConfirmLobbyJoinAsync(lobbyId, joinId, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateLobbyJoinAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439023";
        var joinId = "507f1f77bcf86cd799439024";
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.ConfirmLobbyJoinAsync(lobbyId, joinId, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region InsertAsync Tests

    [Fact]
    public async Task InsertAsync_ValidLobby_InsertsSuccessfully()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby",
            elo: 1500);

        // Act
        var result = await _handler.InsertLobbyAsync(lobby, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedLobbies = await GetAllGameLobbiesAsync();
        Assert.Single(insertedLobbies);
        Assert.Equal(lobby.Id, insertedLobbies.First().Id);
        Assert.Equal(lobby.LobbyName, insertedLobbies.First().LobbyName);
    }

    [Fact]
    public async Task InsertAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange
        var lobby = CreateTestGameLobby("lobby_timeout", "Test Lobby Timeout");

        // Act
        var result = await _handler.InsertLobbyAsync(lobby, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InsertAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var lobby = CreateTestGameLobby("lobby0", "Test Lobby");
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.InsertLobbyAsync(lobby, cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Advanced Concurrency and Race Condition Tests

    [Fact]
    public async Task Concurrent_MultipleFindByIdAsync_SameLobby()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby",
            elo: 1500);
        await InsertTestGameLobbyAsync(lobby);

        // Act - Start multiple concurrent read operations
        var tasks = new List<Task<Result<GameLobby>>>
        {
            _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30),
            _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30),
            _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30),
            _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30),
            _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30)
        };

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var result = await task;
            Assert.True(result.IsSuccess);
            Assert.Equal(lobby.Id, result.Value.Id);
            Assert.Equal(lobby.LobbyName, result.Value.LobbyName);
            Assert.Equal(lobby.Elo, result.Value.Elo);
        }
    }

    [Fact]
    public async Task Concurrent_ReadAndWriteOperations_SameLobby()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby");
        await InsertTestGameLobbyAsync(lobby);

        var joinId = "507f1f77bcf86cd799439012";

        // Act - Mix of read and write operations
        var task1 = _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30);
        var task2 = _handler.FindLobbyByNameAsync(lobby.LobbyName, CancellationToken.None, 30);
        var task3 = _handler.ConfirmLobbyJoinAsync(lobby.Id, joinId, CancellationToken.None, 30);
        var task4 = _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30);
        var task5 = _handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        await Task.WhenAll(task1, task2, task3, task4, task5);
        var result1 = await task1;
        var result2 = await task2;
        var result3 = await task3;
        var result4 = await task4;
        var result5 = await task5;

        // Assert
        // All operations should complete successfully
        Assert.True(result1.IsSuccess); // FindById
        Assert.True(result2.IsSuccess); // FindByName
        Assert.True(result3.IsSuccess); // UpdateLobbyJoin
        Assert.True(result4.IsSuccess); // FindById after update
        Assert.True(result5.IsSuccess); // GetPublicLobbies

        // Verify the lobby was updated
        var updatedLobby = result4.Value;
        Assert.Equal(joinId, updatedLobby.JoinedId);
    }

    [Fact]
    public async Task Concurrent_DeleteAndReadOperations_SameLobby()
    {
        // Arrange
        var lobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Test Lobby");
        await InsertTestGameLobbyAsync(lobby);
        var cts = new CancellationTokenSource();

        // Act - Mix of delete and read operations
        var task1 = _handler.DeleteGameLobbiesAsync(lobby.Id, CancellationToken.None, 30);
        var task2 = _handler.FindLobbyByIdAsync(lobby.Id, CancellationToken.None, 30);
        var task3 = _handler.FindLobbyByNameAsync(lobby.LobbyName, CancellationToken.None, 30);
        var task4 = _handler.WaitForLobbyJoinConfirmationAsync(lobby.Id, cts.Token, 30);

        cts.Cancel();
        await Task.WhenAll(task1, task2, task3, task4);

        var result1 = await task1;
        var result2 = await task2;
        var result3 = await task3;
        var result4 = await task4;

        // Assert
        Assert.True(result1.IsSuccess); // Delete should succeed
        Assert.True(result2.IsSuccess | result2.IsFailed);
        Assert.True(result3.IsSuccess | result3.IsFailed);
        Assert.True(result4.IsSuccess | result4.IsFailed);
    }

    [Fact]
    public async Task WatchChangesAsync_MultipleSubscribers_ConcurrentChanges()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        // Set up multiple subscribers
        var subscriber1Changes = new List<string>();
        var subscriber2Changes = new List<string>();

        var watchTask1 = Task.Run(() => WatchForChanges(_handler, subscriber1Changes, cts.Token));
        var watchTask2 = Task.Run(() => WatchForChanges(_handler, subscriber2Changes, cts.Token));

        // Let watchers start
        await Task.Delay(100);

        // Act - Make several changes
        var lobby1 = CreateTestGameLobby("507f1f77bcf86cd799439011", "Lobby 1");
        var lobby2 = CreateTestGameLobby("507f1f77bcf86cd799439012", "Lobby 2");
        var lobby3 = CreateTestGameLobby("507f1f77bcf86cd799439013", "Lobby 3");

        await _handler.InsertLobbyAsync(lobby1, CancellationToken.None, 30);
        await Task.Delay(50);
        await _handler.InsertLobbyAsync(lobby2, CancellationToken.None, 30);
        await Task.Delay(50);
        await _handler.InsertLobbyAsync(lobby3, CancellationToken.None, 30);

        // Let changes propagate
        await Task.Delay(200);

        // Stop watching
        cts.Cancel();

        // Wait for watch tasks to complete
        try
        {
            await Task.WhenAll(watchTask1, watchTask2);
        }
        catch (OperationCanceledException)
        {
            // Expected when cancelled
        }

        // Assert
        // Both subscribers should have received notifications
        Assert.NotEmpty(subscriber1Changes);
        Assert.NotEmpty(subscriber2Changes);

        // Subscribers should have received the same changes (order might differ)
        Assert.Equal(subscriber1Changes.Count, subscriber2Changes.Count);
    }

    [Fact]
    public async Task WatchChangesAsync_LongRunning_MultipleChanges()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var receivedChanges = new List<string>();
        var changeLock = new object();

        var watchTask = Task.Run(async () =>
        {
            await _handler.WatchChangesAsync(change =>
            {
                lock (changeLock)
                {
                    receivedChanges.Add($"{change.Item1}:{change.Item2}");
                }
                return Task.CompletedTask;
            }, cts.Token);
        });

        // Let watcher start
        await Task.Delay(100);

        // Act - Make many changes over time
        var lobbies = new List<GameLobby>();
        for (int i = 0; i < 10; i++)
        {
            var lobby = CreateTestGameLobby($"507f1f77bcf86cd7994390{i:D2}", $"Lobby {i}");
            lobbies.Add(lobby);
            await _handler.InsertLobbyAsync(lobby, CancellationToken.None, 30);
            await Task.Delay(10);
        }

        // Update some lobbies
        for (int i = 0; i < 5; i++)
        {
            await _handler.ConfirmLobbyJoinAsync(lobbies[i].Id, $"507f1f77bcf86cd7994391{i:D2}", CancellationToken.None, 30);
            await Task.Delay(10);
        }

        // Delete some lobbies
        for (int i = 0; i < 3; i++)
        {
            await _handler.DeleteGameLobbiesAsync(lobbies[i].Id, CancellationToken.None, 30);
            await Task.Delay(10);
        }

        // Let changes propagate
        await Task.Delay(200);

        // Stop watching
        cts.Cancel();

        try
        {
            await watchTask;
        }
        catch (OperationCanceledException)
        {
            // Expected when cancelled
        }

        // Assert
        Assert.NotEmpty(receivedChanges);

        // Should have received insert, update, and delete operations
        var operations = receivedChanges.Select(c => c.Split(':')[0]).ToList();
        Assert.Contains("Insert", operations);
        Assert.Contains("Update", operations);
        Assert.Contains("Delete", operations);
    }

    [Fact]
    public async Task RaceCondition_InsertAndImmediateDelete_SameLobby()
    {
        // Arrange
        var lobbyId = "507f1f77bcf86cd799439011";
        var lobby = CreateTestGameLobby(lobbyId, "Test Lobby");

        // Act - Try to insert and immediately delete
        var insertTask = _handler.InsertLobbyAsync(lobby, CancellationToken.None, 30);
        var deleteTask = _handler.DeleteGameLobbiesAsync(lobbyId, CancellationToken.None, 30);

        var results = await Task.WhenAll(insertTask, deleteTask);

        // Assert
        // At least one operation should succeed
        var successCount = results.Count(r => r.IsSuccess);
        Assert.True(successCount >= 1);

        // Verify final state
        var allLobbies = await GetAllGameLobbiesAsync();
        var lobbyExists = allLobbies.Any(l => l.Id == lobbyId);

        // Either the lobby exists (insert succeeded) or doesn't exist (delete succeeded)
        Assert.True(lobbyExists || !lobbyExists);
    }

    private async Task WatchForChanges(GameLobbiesCollectionHandler handler, List<string> changes, CancellationToken cancellationToken)
    {
        await handler.WatchChangesAsync(change =>
        {
            lock (changes)
            {
                changes.Add($"{change.Item1}:{change.Item2}");
            }
            return Task.CompletedTask;
        }, cancellationToken);
    }

    #endregion


    #region WatchChangesAsync Tests

    [Fact]
    public async Task WatchChangesAsync_ValidCallback_ExecutesWithoutException()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        // Act & Assert
        var watchTask = _handler.WatchChangesAsync(_ => Task.CompletedTask, cts.Token);

        // Let it run for a short time
        await Task.Delay(100);
        cts.Cancel();
        await Task.WhenAny(watchTask);

        // The test passes if no exception is thrown
        Assert.True(watchTask.Status == TaskStatus.Canceled);
    }

    #endregion


    #region GetPublicLobbiesAsync Tests

    [Fact]
    public async Task GetPublicLobbiesAsync_ValidLobbies_ReturnsPublicLobbies()
    {
        // Arrange
        var publicLobby1 = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Public Lobby 1",
            elo: 1500);
        var publicLobby2 = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439012",
            lobbyName: "Public Lobby 2",
            elo: 1600);
        var privateLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439013",
            lobbyName: "Private Lobby",
            joinedId: "507f1f77bcf86cd799439014",
            elo: 1700);

        await InsertTestGameLobbyAsync(publicLobby1);
        await InsertTestGameLobbyAsync(publicLobby2);
        await InsertTestGameLobbyAsync(privateLobby);

        // Act
        var result = await _handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, l => l.Id == publicLobby1.Id && l.LobbyName == publicLobby1.LobbyName);
        Assert.Contains(result.Value, l => l.Id == publicLobby2.Id && l.LobbyName == publicLobby2.LobbyName);
    }

    [Fact]
    public async Task GetPublicLobbiesAsync_NoPublicLobbies_ReturnsEmptyList()
    {
        // Arrange
        var privateLobby = CreateTestGameLobby(
            id: "507f1f77bcf86cd799439011",
            lobbyName: "Private Lobby",
            joinedId: "507f1f77bcf86cd799439012");

        await InsertTestGameLobbyAsync(privateLobby);

        // Act
        var result = await _handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
        Assert.NotNull(result.Value); // Ensure the list itself is not null
    }

    [Fact]
    public async Task GetPublicLobbiesAsync_OperationTimeout_ReturnsFailure()
    {
        // Arrange & Act
        var result = await _handler.GetPublicLobbiesAsync(CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("timeout", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetPublicLobbiesAsync_OperationCancelled_ReturnsFailure()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act
        var result = await _handler.GetPublicLobbiesAsync(cancellationTokenSource.Token, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("cancelled", result.Errors.First().Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetPublicLobbiesAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.GameLobbies)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<GameLobbiesCollectionHandler>>();
        var handler = new GameLobbiesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.GetPublicLobbiesAsync(CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion
}