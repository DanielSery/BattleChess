using AwesomeAssertions;
using CrownsGuard.Database.Database;
using CrownsGuard.Database.Ranked;
using CrownsGuard.Database.Utilities;
using FluentResults;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Database.Test.Ranked;

public class RankedGamesCollectionHandlerTest : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly IMongoCollection<RankedGame> _rankedGamesCollection;
    private readonly RankedGamesCollectionHandler _handler;

    public RankedGamesCollectionHandlerTest()
    {
        // Start MongoDB test instance
        _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);

        // Create test database and collection
        var client = new MongoClient(_mongoRunner.ConnectionString);
        var database = client.GetDatabase("TestBattleChess");
        _rankedGamesCollection = database.GetCollection<RankedGame>("RankedGames");

        // Create a mock database client for testing
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGames).Returns(_rankedGamesCollection);
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        // Create a mock logger for testing
        var mockLogger = new Mock<ILogger<RankedGamesCollectionHandler>>();

        // Create handler with mocked dependencies
        _handler = new RankedGamesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);
    }

    public void Dispose()
    {
        _mongoRunner.Dispose();
        GC.SuppressFinalize(this);
    }

    #region Helper Methods

    private RankedGame CreateTestRankedGame(
        string id,
        string? playerId = null,
        string? joinedId = null,
        int? version = null,
        short? elo = null,
        bool isHostStarting = false,
        int[]? map = null)
    {
        return new RankedGame
        {
            Id = id,
            PlayerId = playerId,
            JoinedId = joinedId,
            Version = version ?? GameVersion.VersionId,
            Elo = elo ?? 1500,
            IsHostStarting = isHostStarting,
            Map = map ?? new[] { 0, 1, 2, 3, 4 }
        };
    }

    private async Task<List<RankedGame>> GetAllRankedGamesAsync()
    {
        return await _rankedGamesCollection.Find(_ => true).ToListAsync();
    }

    private async Task InsertTestRankedGameAsync(RankedGame rankedGame)
    {
        await _rankedGamesCollection.InsertOneAsync(rankedGame);
    }

    #endregion
    #region ConfirmGameAsync Tests

    [Fact]
    public async Task ConfirmGameAsync_ValidGame_UpdatesJoinId()
    {
        // Arrange
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            playerId: "507f1f77bcf86cd799439012",
            elo: 1500);
        await InsertTestRankedGameAsync(game);
        var joinId = "507f1f77bcf86cd799439013";

        // Act
        var result = await _handler.ConfirmGameAsync(game.Id, joinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var updatedGames = await GetAllRankedGamesAsync();
        var updatedGame = updatedGames.FirstOrDefault(g => g.Id == game.Id);
        Assert.NotNull(updatedGame);
        Assert.Equal(joinId, updatedGame.JoinedId);
    }

    [Fact]
    public async Task ConfirmGameAsync_InvalidGame_ReturnsFailure()
    {
        // Arrange
        var invalidGameId = "507f1f77bcf86cd799439011";
        var joinId = "507f1f77bcf86cd799439012";

        // Act
        var result = await _handler.ConfirmGameAsync(invalidGameId, joinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }


    [Fact]
    public async Task ConfirmGameAsync_DatabaseException_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGames)
            .Throws(new MongoException("Database connection failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<RankedGamesCollectionHandler>>();
        var handler = new RankedGamesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.ConfirmGameAsync("507f1f77bcf86cd799439018", "507f1f77bcf86cd799439019", CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database connection failed", result.Errors.First().Message);
    }

    #endregion
    #region DeleteGameSearchAsync Tests

    [Fact]
    public async Task DeleteGameSearchAsync_ValidGameId_DeletesGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var game1 = CreateTestRankedGame(gameId, elo: 1500);
        var game2 = CreateTestRankedGame("507f1f77bcf86cd799439012", elo: 1600);

        await InsertTestRankedGameAsync(game1);
        await InsertTestRankedGameAsync(game2);

        // Act
        var result = await _handler.DeleteGameSearchAsync(gameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var remainingGames = await GetAllRankedGamesAsync();
        Assert.Single(remainingGames);
        Assert.Equal(game2.Id, remainingGames.First().Id);
    }

    [Fact]
    public async Task DeleteGameSearchAsync_InvalidGameId_ReturnsSuccess()
    {
        // Arrange
        var invalidGameId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _handler.DeleteGameSearchAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
    }


    #endregion
    #region FindGameForTargetEloAsync Tests

    [Fact]
    public async Task FindGameForTargetEloAsync_ImmediateMatch_ReturnsGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010"; // Lower than target for change stream filter
        var targetElo = 1500;
        var eloDifference = 100;

        var matchingGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            playerId: "507f1f77bcf86cd799439012",
            elo: (short)(targetElo + 50),
            joinedId: null);

        await InsertTestRankedGameAsync(matchingGame);

        // Act
        var result = await _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(matchingGame.Id, result.Value.Id);
        Assert.Equal(matchingGame.Elo, result.Value.Elo);
    }

    [Fact]
    public async Task FindGameForTargetEloAsync_ChangeStreamWaiting_ReturnsGame()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010";
        var targetElo = 1500;
        var eloDifference = 100;

        // Act - Start waiting for a game
        var waitTask = _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Insert a matching game after a short delay
        await Task.Delay(100);
        var matchingGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            playerId: "507f1f77bcf86cd799439012",
            elo: (short)(targetElo + 50),
            joinedId: null);

        await InsertTestRankedGameAsync(matchingGame);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(matchingGame.Id, result.Value.Id);
    }

    [Fact]
    public async Task FindGameForTargetEloAsync_NoMatch_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010";
        var targetElo = 1500;
        var eloDifference = 50;

        var nonMatchingGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 2000, // Outside the ELO range
            joinedId: null);

        await InsertTestRankedGameAsync(nonMatchingGame);

        // Act
        var result = await _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 1);

        // Assert
        Assert.True(result.IsFailed);
    }


    #endregion
    #region WaitForGameAcceptAsync Tests

    [Fact]
    public async Task WaitForGameAcceptAsync_ImmediateAcceptance_ReturnsGame()
    {
        // Arrange
        var joinedGameId = "507f1f77bcf86cd799439011";
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            joinedId: joinedGameId);

        await InsertTestRankedGameAsync(game);

        // Act
        var result = await _handler.WaitForGameAcceptAsync(game.Id, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(game.Id, result.Value.Id);
        Assert.Equal(joinedGameId, result.Value.JoinedId);
    }

    [Fact]
    public async Task WaitForGameAcceptAsync_ChangeStreamWaiting_ReturnsGame()
    {
        // Arrange
        var joinedGameId = "507f1f77bcf86cd799439011";
        
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            joinedId: null);

        await InsertTestRankedGameAsync(game);

        // Act - Start waiting for acceptance
        var waitTask = _handler.WaitForGameAcceptAsync(game.Id, CancellationToken.None, 30);

        // Update the game to accept after a short delay
        await Task.Delay(500);

        await _handler.ConfirmGameAsync(game.Id, joinedGameId, CancellationToken.None, 30);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(game.Id, result.Value.Id);
    }

    [Fact]
    public async Task WaitForGameAcceptAsync_GameDeleted_ReturnsFailure()
    {
        // Arrange
        var joinedGameId = "507f1f77bcf86cd799439011";
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            joinedId: joinedGameId);

        await InsertTestRankedGameAsync(game);

        // Act
        var waitTask = _handler.WaitForGameAcceptAsync(joinedGameId, CancellationToken.None, 30);

        // Delete the game while waiting
        await _handler.DeleteGameSearchAsync(game.Id, CancellationToken.None, 30);

        var result = await waitTask;

        // Assert
        Assert.True(result.IsFailed);
    }


    #endregion
    #region InsertAsync Tests

    [Fact]
    public async Task InsertAsync_ValidGame_InsertsSuccessfully()
    {
        // Arrange
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            playerId: "507f1f77bcf86cd799439012",
            elo: 1500);

        // Act
        var result = await _handler.InsertAsync(game, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        var insertedGames = await GetAllRankedGamesAsync();
        Assert.Single(insertedGames);
        Assert.Equal(game.Id, insertedGames.First().Id);
        Assert.Equal(game.PlayerId, insertedGames.First().PlayerId);
        Assert.Equal(game.Elo, insertedGames.First().Elo);
    }

    [Fact]
    public async Task InsertAsync_DuplicateId_ReturnsFailure()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var game1 = CreateTestRankedGame(gameId, playerId: "507f1f77bcf86cd799439012", elo: 1500);
        var game2 = CreateTestRankedGame(gameId, playerId: "507f1f77bcf86cd799439013", elo: 1600);

        // Act
        await _handler.InsertAsync(game1, CancellationToken.None, 30);
        var result = await _handler.InsertAsync(game2, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task InsertAsync_InvalidData_ReturnsFailure()
    {
        // Arrange
        var game = CreateTestRankedGame("invalid-id", elo: 1500);

        // Act
        var result = await _handler.InsertAsync(game, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }


    #endregion
    #region GetClosestGameSearchAsync Tests

    [Fact]
    public async Task GetClosestGameSearchAsync_ExactMatch_ReturnsGame()
    {
        // Arrange
        var searchedElo = 1500;

        var exactMatchGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 1500,
            joinedId: null);

        var otherGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            elo: 1600,
            joinedId: null);

        await InsertTestRankedGameAsync(exactMatchGame);
        await InsertTestRankedGameAsync(otherGame);

        // Act
        var result = await _handler.GetClosestGameSearchAsync(searchedElo, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(exactMatchGame.Id, result.Value.Id);
        Assert.Equal(exactMatchGame.Elo, result.Value.Elo);
    }

    [Fact]
    public async Task GetClosestGameSearchAsync_ClosestMatch_ReturnsClosestGame()
    {
        // Arrange
        var searchedElo = 1500;

        var game1 = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 1450, // 50 difference
            joinedId: null);

        var game2 = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            elo: 1550, // 50 difference
            joinedId: null);

        var game3 = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439013",
            elo: 2000, // Outside range
            joinedId: null);

        await InsertTestRankedGameAsync(game1);
        await InsertTestRankedGameAsync(game2);
        await InsertTestRankedGameAsync(game3);

        // Act
        var result = await _handler.GetClosestGameSearchAsync(searchedElo, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        // Should return either game1 or game2 (both have same difference)
        Assert.True(result.Value.Elo == 1450 || result.Value.Elo == 1550);
    }

    [Fact]
    public async Task GetClosestGameSearchAsync_NoMatchesParameters_ReturnsFailure()
    {
        // Arrange
        var searchedElo = int.MaxValue;

        // Act
        var result = await _handler.GetClosestGameSearchAsync(searchedElo, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
    }

    [Fact]
    public async Task GetClosestGameSearchAsync_AggregationFailure_ReturnsFailure()
    {
        // Arrange
        var mockDatabaseClient = new Mock<IDatabaseClient>();
        mockDatabaseClient.Setup(db => db.RankedGames)
            .Throws(new MongoException("Aggregation pipeline failed"));
        mockDatabaseClient.Setup(db => db.GetServerTimeAsync()).ReturnsAsync(DateTime.UtcNow);

        var mockLogger = new Mock<ILogger<RankedGamesCollectionHandler>>();
        var handler = new RankedGamesCollectionHandler(mockDatabaseClient.Object, mockLogger.Object);

        // Act
        var result = await handler.GetClosestGameSearchAsync(1500, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Aggregation pipeline failed", result.Errors.First().Message);
    }

    #endregion

    #region Enhanced Error Message Validation Tests

    [Fact]
    public async Task ConfirmGameAsync_InvalidObjectIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var invalidGameId = "invalid-object-id-format";
        var joinId = "507f1f77bcf86cd799439012";

        // Act
        var result = await _handler.ConfirmGameAsync(invalidGameId, joinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    [Fact]
    public async Task ConfirmGameAsync_InvalidJoinIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439011";
        var invalidJoinId = "invalid-object-id-format";

        // Act
        var result = await _handler.ConfirmGameAsync(gameId, invalidJoinId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    [Fact]
    public async Task DeleteGameSearchAsync_InvalidObjectIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var invalidGameId = "invalid-object-id-format";

        // Act
        var result = await _handler.DeleteGameSearchAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    [Fact]
    public async Task FindGameForTargetEloAsync_InvalidGameIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var invalidGameId = "invalid-object-id-format";
        var targetElo = 1500;
        var eloDifference = 100;

        // Act
        var result = await _handler.FindGameForTargetEloAsync(invalidGameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    [Fact]
    public async Task WaitForGameAcceptAsync_InvalidGameIdFormat_ReturnsObjectIdParseError()
    {
        // Arrange
        var invalidGameId = "invalid-object-id-format";

        // Act
        var result = await _handler.WaitForGameAcceptAsync(invalidGameId, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to parse ObjectId", result.Errors.First().Message);
    }

    #endregion

    #region Comprehensive Input Validation Tests


    [Fact]
    public async Task GetClosestGameSearchAsync_NegativeElo_ReturnsFailure()
    {
        // Act
        var result = await _handler.GetClosestGameSearchAsync(-100, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Single(result.Errors);
        Assert.NotEmpty(result.Errors.First().Message);
    }


    #endregion

    #region Advanced Boundary Condition Tests

    [Fact]
    public async Task FindGameForTargetEloAsync_MaximumEloBoundary_TestEdgeCases()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010";
        var targetElo = short.MaxValue;
        var eloDifference = 100;

        var maxEloGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: short.MaxValue,
            joinedId: null);

        var nearMaxEloGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            elo: (short)(short.MaxValue - 50),
            joinedId: null);

        await InsertTestRankedGameAsync(maxEloGame);
        await InsertTestRankedGameAsync(nearMaxEloGame);

        // Act
        var result = await _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Elo == short.MaxValue || result.Value.Elo == (short.MaxValue - 50));
    }

    [Fact]
    public async Task TimeoutBoundaryTests_MinimumTimeout_ReturnsTimeoutError()
    {
        // Arrange
        var game = CreateTestRankedGame("507f1f77bcf86cd799439011", elo: 1500);

        // Act
        var result = await _handler.InsertAsync(game, CancellationToken.None, 0);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Operation timeout", result.Errors.First().Message);
    }

    [Fact]
    public async Task ConcurrentOperations_ChangeStreamRaceCondition()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010";
        var targetElo = 1500;
        var eloDifference = 100;

        // Start waiting for a game
        var waitTask = _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Insert multiple games concurrently
        var game1 = CreateTestRankedGame("507f1f77bcf86cd799439011", elo: 1500, joinedId: null);
        var game2 = CreateTestRankedGame("507f1f77bcf86cd799439012", elo: 1500, joinedId: null);
        var game3 = CreateTestRankedGame("507f1f77bcf86cd799439013", elo: 1500, joinedId: null);

        var insertTask1 = InsertTestRankedGameAsync(game1);
        var insertTask2 = InsertTestRankedGameAsync(game2);
        var insertTask3 = InsertTestRankedGameAsync(game3);

        await Task.WhenAll(insertTask1, insertTask2, insertTask3);

        // Act
        var result = await waitTask;

        // Assert
        Assert.True(result.IsSuccess);
        // Should return one of the inserted games
        var allGames = await GetAllRankedGamesAsync();
        var returnedGame = allGames.FirstOrDefault(g => g.Id == result.Value.Id);
        Assert.NotNull(returnedGame);
    }

    #endregion

    #region Integration and Interaction Tests

    [Fact]
    public async Task ComplexWorkflow_CompleteGameLifecycle()
    {
        // This test simulates a complete workflow of game operations

        // Arrange
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            playerId: "507f1f77bcf86cd799439012",
            elo: 1500);

        // Act & Assert - Complete workflow
        // 1. Insert game
        var insertResult = await _handler.InsertAsync(game, CancellationToken.None, 30);
        Assert.True(insertResult.IsSuccess);

        // 2. Find the game
        var findResult = await _handler.GetClosestGameSearchAsync(1500, CancellationToken.None, 30);
        Assert.True(findResult.IsSuccess);
        Assert.Equal(game.Id, findResult.Value.Id);

        // 3. Wait for another player to join
        var waitTask = _handler.WaitForGameAcceptAsync(game.Id, CancellationToken.None, 30);

        // 4. Confirm the game (simulating another player joining)
        var confirmResult = await _handler.ConfirmGameAsync(game.Id, "507f1f77bcf86cd799439013", CancellationToken.None, 30);
        Assert.True(confirmResult.IsSuccess);

        // 5. Verify the wait operation completes
        var waitResult = await waitTask;
        Assert.True(waitResult.IsSuccess);
        Assert.Equal(game.Id, waitResult.Value.Id);
        Assert.Equal("507f1f77bcf86cd799439013", waitResult.Value.JoinedId);

        // 6. Clean up - delete the game
        var deleteResult = await _handler.DeleteGameSearchAsync(game.Id, CancellationToken.None, 30);
        Assert.True(deleteResult.IsSuccess);

        // 7. Verify final state
        var finalGames = await GetAllRankedGamesAsync();
        Assert.DoesNotContain(finalGames, g => g.Id == game.Id);
    }

    #endregion

    #region Additional Boundary Condition Tests

    [Fact]
    public async Task FindGameForTargetEloAsync_BoundaryEloValues_ReturnsCorrectResults()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010";
        var targetElo = 100;
        var eloDifference = 50;

        var minRangeGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 50, // At the lower boundary
            joinedId: null);

        var maxRangeGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            elo: 150, // At the upper boundary
            joinedId: null);

        await InsertTestRankedGameAsync(minRangeGame);
        await InsertTestRankedGameAsync(maxRangeGame);

        // Act
        var result = await _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        // Should return either game (both are within range)
        Assert.True(result.Value.Elo == 50 || result.Value.Elo == 150);
    }

    [Fact]
    public async Task GetClosestGameSearchAsync_BoundaryEloValues_ReturnsCorrectResults()
    {
        // Arrange
        var searchedElo = 0;

        var zeroEloGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 0,
            joinedId: null);

        var maxValueGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            elo: short.MaxValue,
            joinedId: null);

        await InsertTestRankedGameAsync(zeroEloGame);
        await InsertTestRankedGameAsync(maxValueGame);

        // Act
        var result = await _handler.GetClosestGameSearchAsync(searchedElo, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(zeroEloGame.Id, result.Value.Id);
        Assert.Equal(zeroEloGame.Elo, result.Value.Elo);
    }

    [Fact]
    public async Task InsertAsync_GameWithWrongVersion_NotReturnedInQueries()
    {
        // Arrange
        var correctVersionGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            version: GameVersion.VersionId,
            elo: 1500,
            joinedId: null);

        var wrongVersionGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            version: GameVersion.VersionId + 1,
            elo: 1500,
            joinedId: null);

        await InsertTestRankedGameAsync(correctVersionGame);
        await InsertTestRankedGameAsync(wrongVersionGame);

        // Act
        var result = await _handler.GetClosestGameSearchAsync(1500, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(correctVersionGame.Id, result.Value.Id);
    }

    [Fact]
    public async Task FindGameForTargetEloAsync_GameWithJoinedId_NotReturned()
    {
        // Arrange
        var gameId = "507f1f77bcf86cd799439010";
        var targetElo = 1500;
        var eloDifference = 100;

        var availableGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 1500,
            joinedId: null);

        var occupiedGame = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439012",
            elo: 1500,
            joinedId: "507f1f77bcf86cd799439013"); // Already has a player

        await InsertTestRankedGameAsync(availableGame);
        await InsertTestRankedGameAsync(occupiedGame);

        // Act
        var result = await _handler.FindGameForTargetEloAsync(gameId, (short)targetElo, eloDifference, CancellationToken.None, 30);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(availableGame.Id, result.Value.Id);
    }

    #endregion

    #region Concurrency Tests

    [Fact]
    public async Task Concurrent_MultipleConfirmGameAsync_SameGame()
    {
        // Arrange
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 1500);
        await InsertTestRankedGameAsync(game);

        var joinId1 = "507f1f77bcf86cd799439012";
        var joinId2 = "507f1f77bcf86cd799439013";

        // Act - Start both operations concurrently
        var task1 = _handler.ConfirmGameAsync(game.Id, joinId1, CancellationToken.None, 30);
        var task2 = _handler.ConfirmGameAsync(game.Id, joinId2, CancellationToken.None, 30);

        await Task.WhenAll(task1, task2);

        // Assert
        // Verify the game was updated with one of the join IDs
        var findResult = await GetAllRankedGamesAsync();
        var updatedGame = findResult.FirstOrDefault(g => g.Id == game.Id);
        Assert.NotNull(updatedGame);
        Assert.True(updatedGame.JoinedId == joinId1 || updatedGame.JoinedId == joinId2);
    }

    [Fact]
    public async Task Concurrent_ReadAndWriteOperations_SameGame()
    {
        // Arrange
        var game = CreateTestRankedGame(
            id: "507f1f77bcf86cd799439011",
            elo: 1500);
        await InsertTestRankedGameAsync(game);

        var joinId = "507f1f77bcf86cd799439012";

        // Act - Mix of read and write operations
        var task1 = _handler.GetClosestGameSearchAsync(1500, CancellationToken.None, 30);
        var task2 = _handler.ConfirmGameAsync(game.Id, joinId, CancellationToken.None, 30);
        var task3 = _handler.GetClosestGameSearchAsync(1500, CancellationToken.None, 30);

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