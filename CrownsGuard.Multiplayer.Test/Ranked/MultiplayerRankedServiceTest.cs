using AwesomeAssertions;
using CrownsGuard.Database.Ranked;
using CrownsGuard.Multiplayer.Ranked;
using FluentResults;
using JetBrains.Annotations;
using Moq;

namespace CrownsGuard.Multiplayer.Test.Ranked;

[TestSubject(typeof(MultiplayerRankedService))]
public class MultiplayerRankedServiceTest
{
    private readonly Mock<IRankedGameJoinsCollectionHandler> _gameJoins;
    private readonly Mock<IRankedGamesCollectionHandler> _gameRequests;
    
    private readonly MultiplayerRankedService _underTest;

    public MultiplayerRankedServiceTest()
    {
        _gameJoins = new Mock<IRankedGameJoinsCollectionHandler>();
        _gameRequests = new Mock<IRankedGamesCollectionHandler>();

        _underTest = new MultiplayerRankedService(_gameJoins.Object, _gameRequests.Object);
    }
    
    [Fact]
    public async Task TryToJoinGameAsync_WhenInsertFails_ReturnsError()
    {
        // Arrange
        _gameJoins.Setup(x => x.InsertGameJoinAsync(It.IsAny<RankedGameJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail("Database error")));
        
        // Act
        var result = await _underTest.TryToJoinGameAsync("gameId", "playerId", [], CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Failed to insert game join", result.Errors[0].Message);
    }

    [Fact]
    public async Task TryToJoinGameAsync_WhenConfirmError_ReturnsError()
    {
        // Arrange
        _gameJoins.Setup(x => x.InsertGameJoinAsync(It.IsAny<RankedGameJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        _gameRequests.Setup(x => x.WaitForGameConfirmationAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Fail<RankedGame>("Database error")));

        // Act
        var result = await _underTest.TryToJoinGameAsync("gameId", "playerId", [], CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("Database error", result.Errors[0].Message);
        _gameJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
        _gameRequests.Verify(x => x.DeleteGameSearchAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task TryToJoinGameAsync_WhenNotConfirmed_ReturnsError()
    {
        // Arrange
        _gameJoins.Setup(x => x.InsertGameJoinAsync(It.IsAny<RankedGameJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        _gameRequests.Setup(x => x.WaitForGameConfirmationAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame
            {
                Id = "gameId",
                Map = [],
                JoinedId = null,
                PlayerId = "playerId",
            })));
        
        // Act
        var result = await _underTest.TryToJoinGameAsync("gameId", "playerId", [], CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("The ranked game was invalid", result.Errors[0].Message);
        _gameJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
        _gameRequests.Verify(x => x.DeleteGameSearchAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task TryToJoinGameAsync_WhenLobbyFull_ReturnsError()
    {
        // Arrange
        _gameJoins.Setup(x => x.InsertGameJoinAsync(It.IsAny<RankedGameJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        _gameRequests.Setup(x => x.WaitForGameConfirmationAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame
            {
                Id = "gameId",
                Map = [],
                JoinedId = "otherJoinedId",
                PlayerId = "playerId",
            }
            )));
        
        // Act
        var result = await _underTest.TryToJoinGameAsync("gameId", "playerId", [], CancellationToken.None);
        
        // Assert
        Assert.True(result.IsFailed);
        Assert.Contains("The lobby is already full", result.Errors[0].Message);
        _gameJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
        _gameRequests.Verify(x => x.DeleteGameSearchAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task TryToJoinGameAsync_WhenOk_ReturnsOk()
    {
        // Arrange
        _gameJoins.Setup(x => x.InsertGameJoinAsync(It.IsAny<RankedGameJoin>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Callback<RankedGameJoin, CancellationToken, int>((gameJoin, _, _) => gameJoin.Id = "joinId")
            .Returns(Task.FromResult(Result.Ok()));
        _gameRequests.Setup(x => x.WaitForGameConfirmationAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame
            {
                Id = "gameId",
                Map = [],
                JoinedId = "joinId",
                PlayerId = "playerId",
            })));
        
        // Act
        var result = await _underTest.TryToJoinGameAsync("gameId", "playerId", [], CancellationToken.None);
        
        // Assert
        Assert.True(result.IsSuccess);
        _gameJoins.Verify(x => x.DeleteGameJoinsAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
        _gameRequests.Verify(x => x.DeleteGameSearchAsync("gameId", It.IsAny<CancellationToken>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task WaitForGameSearchOrJoinAsync_WhenFoundGameSearch_ReturnsGameSearch()
    {
        // Arrange
        _gameJoins.Setup(x => x.WaitForGameJoinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns<string, CancellationToken, int>(async (_, token, _) =>
            {
                await Task.Delay(TimeSpan.FromDays(1), token);
                return Result.Fail("Timeout");
            });
        _gameRequests.Setup(x => x.FindGameForTargetEloAsync("gameId", 1000, 100, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame
            {
                Map = [],
                PlayerId = "playerId",
            })));
        
        // Act
        var result = await _underTest.WaitForGameFindOrJoinAsync(
            "gameId",
            1000,
            100,
            30,
            CancellationToken.None);
        
        // Assert
        result.result.Should().Be(IMultiplayerRankedService.WaitResult.GameSearch);
        result.search.Should().NotBe(null);
        result.searchJoin.Should().Be(null);
    }

    [Fact]
    public async Task WaitForGameSearchOrJoinAsync_WhenFoundGameJoin_ReturnsGameJoin()
    {
        // Arrange
        _gameJoins.Setup(x => x.WaitForGameJoinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGameJoin
            {
                GameId = "gameId",
                Map = []
            })));
        _gameRequests.Setup(x => x.FindGameForTargetEloAsync("gameId", 1000, 100, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns<string, int, int, CancellationToken, int>(async (_, _, _, token, _) =>
            {
                await Task.Delay(TimeSpan.FromDays(1), token);
                return Result.Fail("Timeout");
            });
        
        // Act
        var result = await _underTest.WaitForGameFindOrJoinAsync(
            "gameId",
            1000,
            100,
            30,
            CancellationToken.None);
        
        // Assert
        result.result.Should().Be(IMultiplayerRankedService.WaitResult.GameJoin);
        result.search.Should().Be(null);
        result.searchJoin.Should().NotBe(null);
    }

    [Fact]
    public async Task WaitForGameSearchOrJoinAsync_WhenTimeout_ReturnsTimeout()
    {
        // Arrange
        _gameJoins.Setup(x => x.WaitForGameJoinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns<string, CancellationToken, int>(async (_, token, _) =>
            {
                await Task.Delay(TimeSpan.FromDays(1), token);
                return Result.Fail("Timeout");
            });
        _gameRequests.Setup(x => x.FindGameForTargetEloAsync("gameId", 1000, 100, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns<string, int, int, CancellationToken, int>(async (_, _, _, token, _) =>
            {
                await Task.Delay(TimeSpan.FromDays(1), token);
                return Result.Fail("Timeout");
            });
        
        // Act
        var result = await _underTest.WaitForGameFindOrJoinAsync(
            "gameId",
            1000,
            100,
            0,
            CancellationToken.None);
        
        // Assert
        result.result.Should().Be(IMultiplayerRankedService.WaitResult.Timeout);
        result.search.Should().Be(null);
        result.searchJoin.Should().Be(null);
    }

    [Fact]
    public async Task WaitForGameSearchOrJoinAsync_WhenCancel_ReturnsTimeout()
    {
        // Arrange
        _gameJoins.Setup(x => x.WaitForGameJoinAsync(It.IsAny<string>(), It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns<string, CancellationToken, int>(async (_, token, _) =>
            {
                await Task.Delay(TimeSpan.FromDays(1), token);
                return Result.Fail("Timeout");
            });
        _gameRequests.Setup(x => x.FindGameForTargetEloAsync("gameId", 1000, 100, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns<string, int, int, CancellationToken, int>(async (_, _, _, token, _) =>
            {
                await Task.Delay(TimeSpan.FromDays(1), token);
                return Result.Fail("Timeout");
            });
        
        // Act
        var tokenSource = new CancellationTokenSource();
        await tokenSource.CancelAsync();
        var result = await _underTest.WaitForGameFindOrJoinAsync(
            "gameId",
            1000,
            100,
            30,
            tokenSource.Token);
        
        // Assert
        result.result.Should().Be(IMultiplayerRankedService.WaitResult.Timeout);
        result.search.Should().Be(null);
        result.searchJoin.Should().Be(null);
    }
}