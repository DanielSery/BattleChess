using System.Security;
using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Database.Lobby;
using CrownsGuard.Database.Players;
using CrownsGuard.Multiplayer.Lobby;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Utilities;
using FluentResults;
using JetBrains.Annotations;
using MongoDB.Driver;
using Moq;

namespace CrownsGuard.Multiplayer.Test.Lobby;

[TestSubject(typeof(MultiplayerLobbyService))]
public class MultiplayerLobbyServiceTest
{
    private readonly Mock<IGameLobbiesCollectionHandler> _gameLobbies;
    private readonly Mock<IGameLobbyJoinsCollectionHandler> _lobbyJoins;

    private readonly MultiplayerLobbyService _underTest;

    public MultiplayerLobbyServiceTest()
    {
        _gameLobbies = new Mock<IGameLobbiesCollectionHandler>();
        _lobbyJoins = new Mock<IGameLobbyJoinsCollectionHandler>();

        _underTest = new MultiplayerLobbyService(
            _gameLobbies.Object,
            _lobbyJoins.Object);
    }

    [Fact]
    public async Task CreateLobbyAsync_WhenLobbyWithNameExists_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;

        _gameLobbies.Setup(x => x.FindLobbyByNameAsync("name", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobby
            {
                LobbyName = "name",
                Map = [],
                PasswordHash = string.Empty,
                PasswordSalt = string.Empty
            })));

        // Act
        var result = await _underTest.CreateLobbyAsync(
            "name",
            new SecureString(),
            null, null,
            setup,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("Lobby already exists");
    }

    [Fact]
    public async Task CreateLobbyAsync_WhenLobbyNotExists_InsertsLobby()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;

        _gameLobbies.Setup(x => x.FindLobbyByNameAsync("name", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<GameLobby>("Lobby not found")));
        _gameLobbies.Setup(x => x.InsertLobbyAsync(It.IsAny<GameLobby>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));

        // Act
        var result = await _underTest.CreateLobbyAsync(
            "name",
            new SecureString(),
            null, null,
            setup,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        _gameLobbies.Verify(
            x => x.InsertLobbyAsync(It.Is<GameLobby>(lobby => lobby.LobbyName == "name"), It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task CreateLobbyAsync_WhenInsertFails_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;

        _gameLobbies.Setup(x => x.FindLobbyByNameAsync("name", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<GameLobby>("Lobby not found")));
        _gameLobbies.Setup(x => x.InsertLobbyAsync(It.IsAny<GameLobby>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail("Database error")));

        // Act
        var result = await _underTest.CreateLobbyAsync(
            "name",
            new SecureString(),
            null, null,
            setup,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("Failed to insert game lobby");
    }

    [Fact]
    public async Task WaitForLobbyPlayerAsync_WhenFailedToGetLobbyJoin_ReturnsError()
    {
        // Arrange
        var gameLobby = new GameLobby
        {
            Id = "gameId",
            LobbyName = "name",
            Map = [],
            PasswordHash = string.Empty,
            PasswordSalt = string.Empty
        };
        
        _lobbyJoins.Setup(x => x.WaitForLobbyJoinAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<GameLobbyJoin>("Database error")));
        
        // Act
        var result = await _underTest.WaitForLobbyPlayerAsync(gameLobby, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        _gameLobbies.Verify(x => x.DeleteGameLobbiesAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
        _lobbyJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
        result.Errors[0].Message.Should().Be("Database error");
    }

    [Fact]
    public async Task WaitForLobbyPlayerAsync_WhenFailedToConfirm_DeletesGame()
    {
        // Arrange
        var gameLobby = new GameLobby
        {
            Id = "gameId",
            LobbyName = "name",
            Map = [],
            PasswordHash = string.Empty,
            PasswordSalt = string.Empty
        };
        
        _lobbyJoins.Setup(x => x.WaitForLobbyJoinAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobbyJoin
            {
                Id = "joinId",
                GameId = "gameId",
                Map = [],
                PlayerId = "playerId",
            })));
        
        _gameLobbies.Setup(x => x.ConfirmLobbyJoinAsync("gameId", "joinId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail("Database error")));
        
        // Act
        var result = await _underTest.WaitForLobbyPlayerAsync(gameLobby, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        _gameLobbies.Verify(x => x.DeleteGameLobbiesAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
        _lobbyJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
    }
    
    [Fact]
    public async Task WaitForLobbyPlayerAsync_WhenConfirmed_NotDeletesGame()
    {
        // Arrange
        var gameLobby = new GameLobby
        {
            Id = "gameId",
            LobbyName = "name",
            Map = [],
            PasswordHash = string.Empty,
            PasswordSalt = string.Empty
        };
        
        _lobbyJoins.Setup(x => x.WaitForLobbyJoinAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobbyJoin
            {
                Id = "joinId",
                GameId = "gameId",
                Map = [],
                PlayerId = "playerId",
            })));
        
        _gameLobbies.Setup(x => x.ConfirmLobbyJoinAsync("gameId", "joinId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        
        // Act
        var result = await _underTest.WaitForLobbyPlayerAsync(gameLobby, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        _gameLobbies.Verify(x => x.DeleteGameLobbiesAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
        _lobbyJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenLobbyNotExists_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        _gameLobbies.Setup(x => x.FindLobbyByNameAsync("lobbyName", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<GameLobby>("Lobby not found")));
        
        // Act
        var result = await _underTest.JoinLobbyAsync(
            "lobbyName",
            new SecureString(),
            null,
            setup,
            CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("Lobby not found");
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenPasswordNotMatches_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;

        _gameLobbies.Setup(x => x.FindLobbyByNameAsync("name", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobby
                {
                    Id = "gameId",
                    LobbyName = "name",
                    Map = [],
                    PasswordHash = "hash",
                    PasswordSalt = "salt"
                }
            )));
        
        // Act
        var result = await _underTest.JoinLobbyAsync(
            "name",
            new SecureString(),
            null,
            setup,
            CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("Password does not match");
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenRequestInsertFails_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        var secureString = new SecureString();
        secureString.AppendChar('p');
        secureString.AppendChar('a');
        secureString.AppendChar('s');
        SetupValidLobby("lobbyId", "lobbyName", secureString);
        
        _lobbyJoins.Setup(x => x.InsertLobbyJoinAsync(It.IsAny<GameLobbyJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail("Database error")));
        
        // Act
        var result = await _underTest.JoinLobbyAsync("lobbyName", secureString, null, setup, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("Failed to request lobby join");
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenRequestNotConfirmed_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        var secureString = new SecureString();
        secureString.AppendChar('p');
        SetupValidLobby("lobbyId", "lobbyName", secureString);
        
        _lobbyJoins.Setup(x => x.InsertLobbyJoinAsync(It.IsAny<GameLobbyJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        
        _gameLobbies.Setup(x => x.WaitForLobbyJoinConfirmationAsync("lobbyId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<GameLobby>("Database error")));
        
        // Act
        var result = await _underTest.JoinLobbyAsync("lobbyName", secureString, null, setup, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("Database error");
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenInvalidLobby_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        var secureString = new SecureString();
        secureString.AppendChar('p');
        SetupValidLobby("lobbyId", "lobbyName", secureString);
        
        _lobbyJoins.Setup(x => x.InsertLobbyJoinAsync(It.IsAny<GameLobbyJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        
        _gameLobbies.Setup(x => x.WaitForLobbyJoinConfirmationAsync("lobbyId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobby
                {
                    Id = "gameId",
                    LobbyName = "name",
                    Map = [],
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    JoinedId = null,
                }
            )));
        
        // Act
        var result = await _underTest.JoinLobbyAsync("lobbyName", secureString, null, setup, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("The lobby was invalid");
        _gameLobbies.Verify(x => x.DeleteGameLobbiesAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
        _lobbyJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenLobbyFull_ReturnsError()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        var secureString = new SecureString();
        secureString.AppendChar('p');
        SetupValidLobby("lobbyId", "lobbyName", secureString);
        
        _lobbyJoins.Setup(x => x.InsertLobbyJoinAsync(It.IsAny<GameLobbyJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        
        _gameLobbies.Setup(x => x.WaitForLobbyJoinConfirmationAsync("lobbyId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobby
                {
                    Id = "gameId",
                    LobbyName = "name",
                    Map = [],
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    JoinedId = "other join",
                }
            )));
        
        // Act
        var result = await _underTest.JoinLobbyAsync("lobbyName", secureString, null, setup, CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        result.Errors[0].Message.Should().Be("The lobby is already full");
        _gameLobbies.Verify(x => x.DeleteGameLobbiesAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
        _lobbyJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task JoinLobbyAsync_WhenEverythingIsOk_ReturnsSuccess()
    {
        // Arrange
        var setup = new Figure[16];
        setup[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        
        var secureString = new SecureString();
        secureString.AppendChar('p');
        SetupValidLobby("lobbyId", "lobbyName", secureString);

        _lobbyJoins.Setup(x => x.InsertLobbyJoinAsync(It.IsAny<GameLobbyJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Callback<GameLobbyJoin, CancellationToken, int>((join, _, _) => join.Id = "joinId")
            .Returns(Task.FromResult(Result.Ok()));

        _gameLobbies.Setup(x => x.WaitForLobbyJoinConfirmationAsync("lobbyId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(() => Task.FromResult(Result.Ok(new GameLobby
                {
                    Id = "lobbyId",
                    LobbyName = "name",
                    Map = [],
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    JoinedId = "joinId",
                }
            )));
        
        // Act
        var result = await _underTest.JoinLobbyAsync("lobbyName", secureString, null, setup, CancellationToken.None);
        
        // Assert
         Assert.True(result.IsSuccess);
        _gameLobbies.Verify(x => x.DeleteGameLobbiesAsync("lobbyId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
        _lobbyJoins.Verify(x => x.DeleteGameJoinsAsync("lobbyId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task WatchLobbiesAsync_OnInsert()
    {
        // Arrange
        _gameLobbies.Setup(x => x.WatchChangesAsync(It.IsAny<Func<(ChangeStreamOperationType, string, GameLobby?), Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<(ChangeStreamOperationType, string, GameLobby?), Task>, CancellationToken>((func, t) => 
                func.Invoke((ChangeStreamOperationType.Insert, "added", new GameLobby
                {
                    LobbyName = "lobbyName",
                    Map = [],
                    PasswordSalt = "salt",
                    PasswordHash = "hash"
                })).Wait(t));
            
        // Act
        var addedCalled = false;
        await _underTest.WatchLobbiesAsync(
            _ => addedCalled = true,
            _ => {},
            _ => {},
            CancellationToken.None);
        
        // Assert
        addedCalled.Should().BeTrue();
    }

    [Fact]
    public async Task WatchLobbiesAsync_UpdateExistingLobby_OnUpdate()
    {
        // Arrange
        _gameLobbies.Setup(x => x.WatchChangesAsync(It.IsAny<Func<(ChangeStreamOperationType, string, GameLobby?), Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<(ChangeStreamOperationType, string, GameLobby?), Task>, CancellationToken>((func, t) => 
                func.Invoke((ChangeStreamOperationType.Update, "updated", null)).Wait(t));

        _gameLobbies.Setup(x => x.FindLobbyByIdAsync("updated", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobby
            {
                LobbyName = "lobbyName",
                Map = [],
                PasswordSalt = "salt",
                PasswordHash = "hash"
            })));
        
        // Act
        var updateCalled = false;
        await _underTest.WatchLobbiesAsync(
            _ => {},
            _ => updateCalled = true,
            _ => {},
            CancellationToken.None);
        
        // Assert
        updateCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TaskWatchLobbiesAsync_UpdateNotExistingLobby_NothingCalled()
    {
        // Arrange
        _gameLobbies.Setup(x => x.WatchChangesAsync(It.IsAny<Func<(ChangeStreamOperationType, string, GameLobby?), Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<(ChangeStreamOperationType, string, GameLobby?), Task>, CancellationToken>((func, t) => 
                func.Invoke((ChangeStreamOperationType.Update, "updated", null)).Wait(t));
        
        _gameLobbies.Setup(x => x.FindLobbyByIdAsync("updated", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<GameLobby>("Not found")));
        
        // Act
        var updateCalled = false;
        await _underTest.WatchLobbiesAsync(
            _ => {},
            _ => updateCalled = true,
            _ => {},
            CancellationToken.None);
        
        // Assert
        updateCalled.Should().BeFalse();
    }

    [Fact]
    public async Task TaskWatchLobbiesAsync_DeleteLobby_OnDelete()
    {
        // Arrange
        _gameLobbies.Setup(x => x.WatchChangesAsync(It.IsAny<Func<(ChangeStreamOperationType, string, GameLobby?), Task>>(), It.IsAny<CancellationToken>()))
            .Callback<Func<(ChangeStreamOperationType, string, GameLobby?), Task>, CancellationToken>((func, t) => 
                func.Invoke((ChangeStreamOperationType.Delete, "deleted", null)).Wait(t));
        
        // Act
        var deleteCalled = false;
        await _underTest.WatchLobbiesAsync(
            _ => { },
            _ => { },
            _ => deleteCalled = true,
            CancellationToken.None
        );
        
        // Assert
        deleteCalled.Should().BeTrue();
    }
    
    private void SetupValidLobby(string lobbyId, string lobbyName, SecureString secureString)
    {
        var salt = HashingHelper.GetSalt();
        var hash = HashingHelper.GetHash(secureString, salt);
        
        _gameLobbies.Setup(x => x.FindLobbyByNameAsync(lobbyName, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new GameLobby
                {
                    Id = lobbyId,
                    LobbyName = lobbyName,
                    Map = [],
                    PasswordHash = hash,
                    PasswordSalt = salt
                }
            )));
    }
}