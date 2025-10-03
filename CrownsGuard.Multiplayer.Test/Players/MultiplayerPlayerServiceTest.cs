using System.Collections;
using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Players;
using CrownsGuard.Game.Players;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;
using Moq;

namespace CrownsGuard.Multiplayer.Test.Players;

public class MultiplayerPlayerServiceTest
{
    private readonly Mock<IPlayersCollectionHandler> _mockPlayersHandler;
    private readonly MultiplayerPlayerService _service;

    public MultiplayerPlayerServiceTest()
    {
        _mockPlayersHandler = new Mock<IPlayersCollectionHandler>();
        _service = new MultiplayerPlayerService(_mockPlayersHandler.Object);
    }

    #region Helper Methods

    private RegisteredPlayer CreateTestPlayer(
        string id = "507f1f77bcf86cd799439011",
        string name = "TestPlayer",
        string emailHash = "test@example.com_hash",
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
            UnlockedFigures = unlockedFigures ?? UnlockedFigures.DefaultUnlockedFigures,
            Map = map ?? new[] { 0, 1, 2, 3, 4 }
        };
    }

    private List<PublicPlayerData> CreateTestLeaderboard()
    {
        return new List<PublicPlayerData>
        {
            new PublicPlayerData { Name = "Player1", Elo = 2000 },
            new PublicPlayerData { Name = "Player2", Elo = 1800 },
            new PublicPlayerData { Name = "Player3", Elo = 1600 }
        };
    }

    private Figure[] CreateValidMap()
    {
        return new[]
        {
            Figure.King | Figure.IsWhite | Figure.IsKing, 
            Figure.Queen | Figure.IsWhite, 
            Figure.LegionarySword | Figure.IsWhite, 
            Figure.LegionarySword | Figure.IsWhite, 
            Figure.LegionarySword | Figure.IsWhite,
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
            
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
            Figure.Empty,
        };
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_ValidDependencies_SetsPropertiesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(_service);
        Assert.Null(_service.LoggedInPlayer);
    }

    #endregion

    #region GetLeaderboard Tests

    [Fact]
    public async Task GetLeaderboard_NoLoggedInPlayer_ReturnsTopLeaderboard()
    {
        // Arrange
        var expectedLeaderboard = CreateTestLeaderboard();
        _mockPlayersHandler
            .Setup(h => h.GetTopLeaderboardAsync(It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(expectedLeaderboard));

        // Act
        var result = await _service.GetLeaderboard(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedLeaderboard.Count, result.Value.Count);
        _mockPlayersHandler.Verify(h => h.GetTopLeaderboardAsync(CancellationToken.None, 120), Times.Once);
        _mockPlayersHandler.Verify(h => h.GetUserLeaderboardAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetLeaderboard_WithLoggedInPlayer_ReturnsUserLeaderboard()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player); // Simulate login by setting LoggedInPlayer directly

        var expectedLeaderboard = CreateTestLeaderboard();
        _mockPlayersHandler
            .Setup(h => h.GetUserLeaderboardAsync(player.Id, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(expectedLeaderboard));

        // Act
        var result = await _service.GetLeaderboard(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedLeaderboard.Count, result.Value.Count);
        _mockPlayersHandler.Verify(h => h.GetUserLeaderboardAsync(player.Id, CancellationToken.None, 120), Times.Once);
        _mockPlayersHandler.Verify(h => h.GetTopLeaderboardAsync(It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetLeaderboard_PlayersHandlerFails_ReturnsFailure()
    {
        // Arrange
        _mockPlayersHandler
            .Setup(h => h.GetTopLeaderboardAsync(It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<List<PublicPlayerData>>("Database error"));

        // Act
        var result = await _service.GetLeaderboard(CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database error", result.Errors.First().Message);
    }

    #endregion

    #region GetCurrentPlayer Tests

    [Fact]
    public void GetCurrentPlayer_NoLoggedInPlayer_ReturnsDefaultPlayer()
    {
        // Act
        var result = _service.GetCurrentPlayer();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PlayerColor.White, result.PlayerColor);
        Assert.Equal("Red player", result.Name);
        Assert.Null(result.PlayerId);
        Assert.Null(result.Elo);
    }

    [Fact]
    public void GetCurrentPlayer_WithLoggedInPlayer_ReturnsLoggedInPlayerInfo()
    {
        // Arrange
        var player = CreateTestPlayer(name: "LoggedInUser", elo: 1750);
        _service.TryLogin(player); // Simulate login

        // Act
        var result = _service.GetCurrentPlayer();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PlayerColor.White, result.PlayerColor);
        Assert.Equal(player.Name, result.Name);
        Assert.Equal(player.Id, result.PlayerId);
        Assert.Equal(player.Elo, result.Elo);
    }

    #endregion

    #region GetRemotePlayerAsync Tests

    [Fact]
    public async Task GetRemotePlayerAsync_ValidPlayerId_ReturnsRemotePlayer()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        var player = CreateTestPlayer(id: playerId, name: "RemotePlayer", elo: 1600);

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByIdAsync(playerId, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(player));

        // Act
        var result = await _service.GetRemotePlayerAsync(playerId, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var remotePlayer = result.Value;
        Assert.Equal(PlayerColor.Black, remotePlayer.PlayerColor);
        Assert.Equal(player.Name, remotePlayer.Name);
        Assert.Equal(player.Id, remotePlayer.PlayerId);
        Assert.Equal(player.Elo, remotePlayer.Elo);
    }

    [Fact]
    public async Task GetRemotePlayerAsync_PlayerNotFound_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439099";
        _mockPlayersHandler
            .Setup(h => h.FindPlayerByIdAsync(playerId, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<RegisteredPlayer>("Player not found"));

        // Act
        var result = await _service.GetRemotePlayerAsync(playerId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Could not find remote player", result.Errors.First().Message);
    }

    [Fact]
    public async Task GetRemotePlayerAsync_PlayersHandlerFails_ReturnsFailure()
    {
        // Arrange
        var playerId = "507f1f77bcf86cd799439011";
        _mockPlayersHandler
            .Setup(h => h.FindPlayerByIdAsync(playerId, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<RegisteredPlayer>("Database error"));

        // Act
        var result = await _service.GetRemotePlayerAsync(playerId, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Could not find remote player", result.Errors.First().Message);
    }

    #endregion

    #region GetUserSaltAsync Tests

    [Fact]
    public async Task GetUserSaltAsync_ValidUser_ReturnsSalt()
    {
        // Arrange
        var userName = "TestUser";
        var expectedSalt = "test_salt";
        var player = CreateTestPlayer(name: userName, passwordSalt: expectedSalt);

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(userName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(player));

        // Act
        var result = await _service.GetUserSaltAsync(userName, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedSalt, result.Value);
    }

    [Fact]
    public async Task GetUserSaltAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userName = "NonExistentUser";
        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(userName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<RegisteredPlayer>("User not found"));

        // Act
        var result = await _service.GetUserSaltAsync(userName, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Could not find user", result.Errors.First().Message);
    }

    #endregion

    #region TryLoginAsync Tests

    [Fact]
    public async Task TryLoginAsync_ValidCredentials_LogsInSuccessfully()
    {
        // Arrange
        var userName = "TestUser";
        var hash = "correct_hash";
        var player = CreateTestPlayer(name: userName, passwordHash: hash);

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(userName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(player));

        // Act
        var result = await _service.TryLoginAsync(userName, hash, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(_service.LoggedInPlayer);
        Assert.Equal(player.Id, _service.LoggedInPlayer.Id);
        Assert.Equal(player.Name, _service.LoggedInPlayer.Name);
    }

    [Fact]
    public async Task TryLoginAsync_InvalidUserName_ReturnsFailure()
    {
        // Arrange
        var userName = "NonExistentUser";
        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(userName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<RegisteredPlayer>("User not found"));

        // Act
        var result = await _service.TryLoginAsync(userName, "some_hash", CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Incorrect username or password", result.Errors.First().Message);
        Assert.Null(_service.LoggedInPlayer);
    }

    [Fact]
    public async Task TryLoginAsync_InvalidPassword_ReturnsFailure()
    {
        // Arrange
        var userName = "TestUser";
        var player = CreateTestPlayer(name: userName, passwordHash: "correct_hash");

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(userName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(player));

        // Act
        var result = await _service.TryLoginAsync(userName, "wrong_hash", CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Incorrect username or password", result.Errors.First().Message);
        Assert.Null(_service.LoggedInPlayer);
    }

    #endregion

    #region UpdateCurrentPlayerMapAsync Tests

    [Fact]
    public async Task UpdateCurrentPlayerMapAsync_NoLoggedInPlayer_ReturnsFailure()
    {
        // Arrange
        var map = CreateValidMap();

        // Act
        var result = await _service.UpdateCurrentPlayerMapAsync(map, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("No logged in player", result.Errors.First().Message);
        _mockPlayersHandler.Verify(h => h.UpdatePlayerSetupAsync(It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCurrentPlayerMapAsync_InvalidMap_ReturnsFailure()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player);

        var invalidMap = new[] { Figure.King, Figure.King }; // Invalid - duplicate kings

        // Act
        var result = await _service.UpdateCurrentPlayerMapAsync(invalidMap, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Trying to save setup with not unlocked figures", result.Errors.First().Message);
        _mockPlayersHandler.Verify(h => h.UpdatePlayerSetupAsync(It.IsAny<string>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCurrentPlayerMapAsync_ValidMap_UpdatesSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player);

        var validMap = CreateValidMap();
        var mapData = validMap.GetIntData();

        _mockPlayersHandler
            .Setup(h => h.UpdatePlayerSetupAsync(player.Id, mapData, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _service.UpdateCurrentPlayerMapAsync(validMap, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockPlayersHandler.Verify(h => h.UpdatePlayerSetupAsync(player.Id, mapData, CancellationToken.None, 120), Times.Once);
        Assert.Equal(mapData, _service.LoggedInPlayer?.Map);
    }

    [Fact]
    public async Task UpdateCurrentPlayerMapAsync_PlayersHandlerFails_ReturnsFailure()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player);

        var validMap = CreateValidMap();
        var mapData = validMap.GetIntData();

        _mockPlayersHandler
            .Setup(h => h.UpdatePlayerSetupAsync(player.Id, mapData, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail("Database error"));

        // Act
        var result = await _service.UpdateCurrentPlayerMapAsync(validMap, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database error", result.Errors.First().Message);
    }

    #endregion

    #region UpdateCurrentPlayerUnlockedFigure Tests

    [Fact]
    public async Task UpdateCurrentPlayerUnlockedFigure_NoLoggedInPlayer_ReturnsFailure()
    {
        // Arrange
        var figure = Figure.Queen;

        // Act
        var result = await _service.UpdateCurrentPlayerUnlockedFigure(figure, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("No logged in player", result.Errors.First().Message);
        _mockPlayersHandler.Verify(h => h.UpdatePlayerUnlockedFiguresAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCurrentPlayerUnlockedFigure_ValidFigure_UpdatesSuccessfully()
    {
        // Arrange
        var player = CreateTestPlayer(unlockedFigures: new byte[] { 1, 2, 3, 4, 5, 6 });
        _service.TryLogin(player);

        var figureToUnlock = Figure.Queen;
        var expectedUnlockedFigures = new byte[player.UnlockedFigures.Length];
        player.UnlockedFigures.CopyTo(expectedUnlockedFigures, 0);
        new BitArray(expectedUnlockedFigures) { [(int)figureToUnlock] = true } .CopyTo(expectedUnlockedFigures, 0);

        _mockPlayersHandler
            .Setup(h => h.UpdatePlayerUnlockedFiguresAsync(player.Id, expectedUnlockedFigures, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _service.UpdateCurrentPlayerUnlockedFigure(figureToUnlock, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockPlayersHandler.Verify(h => h.UpdatePlayerUnlockedFiguresAsync(player.Id, expectedUnlockedFigures, CancellationToken.None, 120), Times.Once);
        Assert.Equal(expectedUnlockedFigures, _service.LoggedInPlayer?.UnlockedFigures);
    }

    [Fact]
    public async Task UpdateCurrentPlayerUnlockedFigure_PlayersHandlerFails_ReturnsFailure()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player);

        var figureToUnlock = Figure.Queen;
        var expectedUnlockedFigures = new byte[player.UnlockedFigures.Length];
        player.UnlockedFigures.CopyTo(expectedUnlockedFigures, 0);
        new BitArray(expectedUnlockedFigures) { [(int)figureToUnlock] = true } .CopyTo(expectedUnlockedFigures, 0);

        _mockPlayersHandler
            .Setup(h => h.UpdatePlayerUnlockedFiguresAsync(player.Id, expectedUnlockedFigures, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail("Database error"));

        // Act
        var result = await _service.UpdateCurrentPlayerUnlockedFigure(figureToUnlock, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database error", result.Errors.First().Message);
    }

    #endregion

    #region TryVerifyEmailAsync Tests

    [Fact]
    public async Task TryVerifyEmailAsync_EmailAvailable_ReturnsSuccess()
    {
        // Arrange
        var emailHash = "new_email@example.com_hash";
        _mockPlayersHandler
            .Setup(h => h.HasPlayerWithEmailHashAsync(emailHash, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<bool>("Email not found"));

        // Act
        var result = await _service.TryVerifyEmailAsync(emailHash, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task TryVerifyEmailAsync_EmailAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var emailHash = "existing_email@example.com_hash";
        _mockPlayersHandler
            .Setup(h => h.HasPlayerWithEmailHashAsync(emailHash, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(true));

        // Act
        var result = await _service.TryVerifyEmailAsync(emailHash, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("User with given email already exists", result.Errors.First().Message);
    }

    #endregion

    #region TrySignUpAsync Tests

    [Fact]
    public async Task TrySignUpAsync_ValidData_SignsUpSuccessfully()
    {
        // Arrange
        var name = "NewUser";
        var hash = "password_hash";
        var salt = "password_salt";
        var emailHash = "new_email@example.com_hash";
        var myMap = CreateValidMap();
        var fallbackMap = CreateValidMap();

        _mockPlayersHandler
            .Setup(h => h.HasPlayerWithEmailHashAsync(emailHash, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(false));

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(name, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<RegisteredPlayer>("User not found"));

        _mockPlayersHandler
            .Setup(h => h.InsertPlayerAsync(It.IsAny<RegisteredPlayer>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _service.TrySignUpAsync(name, hash, salt, emailHash, myMap, fallbackMap, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockPlayersHandler.Verify(h => h.InsertPlayerAsync(It.Is<RegisteredPlayer>(p =>
            p.Name == name &&
            p.PasswordHash == hash &&
            p.PasswordSalt == salt &&
            p.EmailHash == emailHash &&
            p.Elo == 1000 &&
            p.Map.SequenceEqual(myMap.GetIntData()) &&
            p.UnlockedFigures.SequenceEqual(UnlockedFigures.DefaultUnlockedFigures)
        ), CancellationToken.None, 120), Times.Once);
    }

    [Fact]
    public async Task TrySignUpAsync_EmailAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var emailHash = "existing_email@example.com_hash";
        _mockPlayersHandler
            .Setup(h => h.HasPlayerWithEmailHashAsync(emailHash, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(true));

        // Act
        var result = await _service.TrySignUpAsync("NewUser", "hash", "salt", emailHash, CreateValidMap(), CreateValidMap(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("User with given email already exists", result.Errors.First().Message);
        _mockPlayersHandler.Verify(h => h.FindPlayerByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
        _mockPlayersHandler.Verify(h => h.InsertPlayerAsync(It.IsAny<RegisteredPlayer>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task TrySignUpAsync_NameAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var name = "ExistingUser";
        var emailHash = "new_email@example.com_hash";

        _mockPlayersHandler
            .Setup(h => h.HasPlayerWithEmailHashAsync(emailHash, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(false));

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(name, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(CreateTestPlayer(name: name)));

        // Act
        var result = await _service.TrySignUpAsync(name, "hash", "salt", emailHash, CreateValidMap(), CreateValidMap(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("User with given name already exists", result.Errors.First().Message);
        _mockPlayersHandler.Verify(h => h.InsertPlayerAsync(It.IsAny<RegisteredPlayer>(), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task TrySignUpAsync_InvalidMap_UsesFallbackMap()
    {
        // Arrange
        var name = "NewUser";
        var emailHash = "new_email@example.com_hash";
        var myMap = new[] { Figure.King, Figure.King }; // Invalid map
        var fallbackMap = CreateValidMap();

        _mockPlayersHandler
            .Setup(h => h.HasPlayerWithEmailHashAsync(emailHash, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(false));

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(name, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Fail<RegisteredPlayer>("User not found"));

        _mockPlayersHandler
            .Setup(h => h.InsertPlayerAsync(It.IsAny<RegisteredPlayer>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok());

        // Act
        var result = await _service.TrySignUpAsync(name, "hash", "salt", emailHash, myMap, fallbackMap, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _mockPlayersHandler.Verify(h => h.InsertPlayerAsync(It.Is<RegisteredPlayer>(p =>
            p.Map.SequenceEqual(fallbackMap.GetIntData())
        ), CancellationToken.None, 120), Times.Once);
    }

    #endregion

    #region Event Tests

    [Fact]
    public async Task Login_ValidCredentials_RaisesLoggedInPlayerChangedEvent()
    {
        // Arrange
        var userName = "TestUser";
        var hash = "correct_hash";
        var player = CreateTestPlayer(name: userName, passwordHash: hash);

        _mockPlayersHandler
            .Setup(h => h.FindPlayerByNameAsync(userName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok(player));

        var eventRaised = false;
        _service.LoggedInPlayerChanged += (_, _) => eventRaised = true;

        // Act
        await _service.TryLoginAsync(userName, hash, CancellationToken.None);

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public async Task UpdateCurrentPlayerMapAsync_Successfully_RaisesLoggedInPlayerChangedEvent()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player);

        var validMap = CreateValidMap();
        var mapData = validMap.GetIntData();

        _mockPlayersHandler
            .Setup(h => h.UpdatePlayerSetupAsync(player.Id, mapData, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok());

        var eventRaised = false;
        _service.LoggedInPlayerChanged += (_, _) => eventRaised = true;

        // Act
        await _service.UpdateCurrentPlayerMapAsync(validMap, CancellationToken.None);

        // Assert
        Assert.True(eventRaised);
    }

    [Fact]
    public async Task UpdateCurrentPlayerUnlockedFigure_Successfully_RaisesLoggedInPlayerChangedEvent()
    {
        // Arrange
        var player = CreateTestPlayer();
        _service.TryLogin(player);

        var figureToUnlock = Figure.Queen;
        var expectedUnlockedFigures = new byte[player.UnlockedFigures.Length];
        player.UnlockedFigures.CopyTo(expectedUnlockedFigures, 0);
        new BitArray(expectedUnlockedFigures) { [(int)figureToUnlock] = true } .CopyTo(expectedUnlockedFigures, 0);

        _mockPlayersHandler
            .Setup(h => h.UpdatePlayerUnlockedFiguresAsync(player.Id, expectedUnlockedFigures, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .ReturnsAsync(Result.Ok());

        var eventRaised = false;
        _service.LoggedInPlayerChanged += (_, _) => eventRaised = true;

        // Act
        await _service.UpdateCurrentPlayerUnlockedFigure(figureToUnlock, CancellationToken.None);

        // Assert
        Assert.True(eventRaised);
    }

    #endregion
}

// Helper extension method to simulate login for testing
internal static class MultiplayerPlayerServiceExtensions
{
    public static void TryLogin(this MultiplayerPlayerService service, RegisteredPlayer? player)
    {
        var loggedInProperty = typeof(MultiplayerPlayerService).GetProperty("LoggedInPlayer",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        loggedInProperty?.SetValue(service, player);
    }
}