using AwesomeAssertions;
using CrownsGuard.Database.Ranked;
using CrownsGuard.Multiplayer.Ranked;
using FluentResults;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;

namespace CrownsGuard.Multiplayer.Test.Ranked;

[TestSubject(typeof(MatchMakingService))]
public class MatchMakingServiceTest
{
    private readonly Mock<IMultiplayerRankedService> _multiplayerService;
    private readonly Mock<IRankedGamesCollectionHandler> _gameRequests;
    private readonly MatchMakingService _underTest;

    public MatchMakingServiceTest()
    {
        _multiplayerService = new Mock<IMultiplayerRankedService>();
        _gameRequests = new Mock<IRankedGamesCollectionHandler>();
        
        _underTest = new MatchMakingService(
            _multiplayerService.Object, 
            _gameRequests.Object,
            Mock.Of<ILogger<IMatchmakingService>>());
    }
    
    [Fact]
    public async Task TryJoinCloseGameAsync_IterativelySearchesWhenGetGame()
    {
        // Arrange
        _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game1", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
            "game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
                    .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game2", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
            })
            .Returns(Task.FromResult(Result.Fail<RankedGameJoin>("Failed to join")));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
                "game2", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
                    .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game3", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
            })
            .Returns(Task.FromResult(Result.Fail<RankedGameJoin>("Failed to join")));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
                "game3", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGameJoin { GameId = "game3", Map = [] })));
    
        // Act
        var result = await _underTest.TryJoinClosestGameAsync("playerId", 1000, 100, [], CancellationToken.None);
    
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.isHost.Should().BeFalse();
        result.Value.gameSearch.Id.Should().Be("game3");
        result.Value.gameSearchJoin.GameId.Should().Be("game3");
        
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Once);
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game2", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Once);
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game3", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Once);
        _gameRequests.Verify(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()), Times.Exactly(3));
    }
    
    [Fact]
    public async Task TryJoinCloseGameAsync_WhenEloIsOutOfSearchedRange_ReturnsError()
    {
        // Arrange
        _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game1", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
                "game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
                    .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game2", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1200})));
            })
            .Returns(Task.FromResult(Result.Fail<RankedGameJoin>("Failed to join")));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
                "game2", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGameJoin { GameId = "game2", Map = [] })));
    
        // Act
        var result = await _underTest.TryJoinClosestGameAsync("playerId", 1000, 100, [], CancellationToken.None);
    
        // Assert
        result.IsFailed.Should().BeTrue();
        
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Once);
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game2", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Never);
        _gameRequests.Verify(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()), Times.Exactly(2));
    }
    
    [Fact]
    public async Task TryJoinCloseGameAsync_WhenNoGameIsFound_ReturnsError()
    {
        // Arrange
        _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game1", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
                "game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
                    .Returns(Task.FromResult(Result.Fail<RankedGame>("Not found game")));
            })
            .Returns(Task.FromResult(Result.Fail<RankedGameJoin>("Failed to join")));
    
        _multiplayerService.Setup(x => x.TryToJoinGameAsync(
                "game2", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGameJoin { GameId = "game2", Map = [] })));
    
        // Act
        var result = await _underTest.TryJoinClosestGameAsync("playerId", 1000, 100, [], CancellationToken.None);
    
        // Assert
        result.IsFailed.Should().BeTrue();
        
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Once);
        _multiplayerService.Verify(x => x.TryToJoinGameAsync("game2", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Never);
        _gameRequests.Verify(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()), Times.Exactly(2));
    }

    [Fact]
    public async Task PerformGameSearchAsync_FindsGame300EloApart()
    {
        // Arrange
        var gameJoin = new RankedGameJoin { Id = "gameJoin", GameId = "gameSearch", Map = [] };
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, Array.Empty<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "gameSearch", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000, IsHostStarting = true})));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 100, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));

        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 150, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 200, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 250, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 300, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.GameJoin, null, gameJoin)));
        
        _gameRequests.Setup(x => x.ConfirmGameJoinAsync("gameSearch", gameJoin.Id, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));

        // Act
        var result = await _underTest.PerformGameSearchAsync("playerId", 1000, 100, [], CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.isHost.Should().BeTrue();
        result.Value.gameSearch.Id.Should().Be("gameSearch");
        result.Value.gameSearchJoin.Id.Should().Be("gameJoin");
        _multiplayerService.Verify(x => x.DeleteGameSearchAsync("gameSearch"), Times.Once);
    }

    [Fact]
    public async Task PerformGameSearchAsync_NotFindsGame350EloApart()
    {
        // Arrange
        var gameJoin = new RankedGameJoin { Id = "gameJoin", GameId = "gameSearch", Map = [] };
        var cancellationToken = new CancellationTokenSource();
        
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, Array.Empty<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "gameSearch", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000, IsHostStarting = true})));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 100, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));

        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 150, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 200, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 250, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 300, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 300, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                    .Callback(() => cancellationToken.Cancel())
                    .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
            })
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 350, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.GameJoin, null, gameJoin)));
        
        _gameRequests.Setup(x => x.ConfirmGameJoinAsync("gameSearch", gameJoin.Id, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));

        // Act
        var result = await _underTest.PerformGameSearchAsync("playerId", 1000, 100, [], cancellationToken.Token);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        _multiplayerService.Verify(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 300, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _multiplayerService.Verify(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 350, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        _multiplayerService.Verify(x => x.DeleteGameSearchAsync("gameSearch"), Times.Once);
    }
    
    [Fact]
    public async Task PerformGameSearchAsync_FindGameJoin_IsHostTrue()
    {
        // Arrange
        var gameJoin = new RankedGameJoin { Id = "gameJoin", GameId = "gameSearch", Map = [] };
        
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, Array.Empty<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "gameSearch", Map = [], JoinedId = "gameJoin", PlayerId = "playerId", Elo = 1000, IsHostStarting = true})));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 100, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.GameJoin, null, gameJoin)));
        
        _gameRequests.Setup(x => x.ConfirmGameJoinAsync("gameSearch", gameJoin.Id, It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));

        // Act
        var result = await _underTest.PerformGameSearchAsync("playerId", 1000, 100, [], CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.isHost.Should().BeTrue();
        result.Value.gameSearch.Id.Should().Be("gameSearch");
        result.Value.gameSearchJoin.Id.Should().Be("gameJoin");
        _multiplayerService.Verify(x => x.DeleteGameSearchAsync("gameSearch"), Times.Never);
    }
    
    [Fact]
    public async Task PerformGameSearchAsync_FindGameSearch_IsHostFalse()
    {
        // Arrange
        var otherGameSearch = new RankedGame { Id = "otherGameSearch", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000, IsHostStarting = false};
        
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, Array.Empty<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "gameSearch", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000, IsHostStarting = true})));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 100, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.GameSearch, otherGameSearch, null)));
        
        _multiplayerService.Setup(x => x.TryToJoinGameAsync("otherGameSearch", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGameJoin { Id = "gameJoin", GameId = "otherGameSearch", Map = [] })));

        // Act
        var result = await _underTest.PerformGameSearchAsync("playerId", 1000, 100, [], CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.isHost.Should().BeFalse();
        result.Value.gameSearch.Id.Should().Be("otherGameSearch");
        result.Value.gameSearchJoin.Id.Should().Be("gameJoin");
        _multiplayerService.Verify(x => x.DeleteGameSearchAsync("gameSearch"), Times.Once);
    }

    [Fact]
    public async Task PerformGameSearchAsync_OnCancelled_ReturnsError()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource();
        await cancellationToken.CancelAsync();
        
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, Array.Empty<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "gameSearch", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000, IsHostStarting = true})));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("gameSearch", 1000, 100, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>((IMultiplayerRankedService.WaitResult.Timeout, null, null)));
        
        // Act
        var result = await _underTest.PerformGameSearchAsync("playerId", 1000, 100, [], cancellationToken.Token);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        _multiplayerService.Verify(x => x.DeleteGameSearchAsync("gameSearch"), Times.Once);
    }

    [Fact]
    public async Task PerformGameSearchAsync_WhenFailsToCreateGameSearch_ReturnsError()
    {
        // Arrange
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, Array.Empty<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Fail<RankedGame>("Failed to create")));
        
        // Act
        var result = await _underTest.PerformGameSearchAsync("playerId", 1000, 100, [], CancellationToken.None);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        _multiplayerService.Verify(x => x.DeleteGameSearchAsync("gameSearch"), Times.Never);
    }

    [Fact]
    public async Task FindRankedGameAsync_WhenConnectsToCloseGame_Ok()
    {
        // Arrange
        _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game1", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
        
        _multiplayerService.Setup(x => x.TryToJoinGameAsync("game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGameJoin { GameId = "game1", Map = [] })));
        
        // Act
        var result = await _underTest.FindRankedGameAsync("playerId", 1000, [], CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _multiplayerService.Verify(x => x.CreateGameSearchAsync("playerId", 1000, It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task FindRankedGameAsync_WhenNotConnectsToCloseGame_CreatesGameSearch()
    {
        // Arrange
        _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game1", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
        
        _multiplayerService.Setup(x => x.TryToJoinGameAsync("game1", "playerId", It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                _gameRequests.Setup(x => x.GetClosestGameSearchAsync(1000, CancellationToken.None, It.IsAny<int>()))
                    .Returns(Task.FromResult(Result.Fail<RankedGame>("Failed to get game")));
            })
            .Returns(Task.FromResult(Result.Fail<RankedGameJoin>("Failed to join")));
        
        _multiplayerService.Setup(x => x.CreateGameSearchAsync("playerId", 1000, It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Result.Ok(new RankedGame { Id = "game2", Map = [], JoinedId = null, PlayerId = "playerId", Elo = 1000})));
        
        _multiplayerService.Setup(x => x.WaitForGameFindOrJoinAsync("game2", 1000, 100, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<(IMultiplayerRankedService.WaitResult, RankedGame?, RankedGameJoin?)>(
                (IMultiplayerRankedService.WaitResult.GameJoin, null, new RankedGameJoin { Id = "gameJoin", GameId = "game2", Map = [] })));
        
        _gameRequests.Setup(x => x.ConfirmGameJoinAsync("game2", "gameJoin", It.IsAny<CancellationToken>(), It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok()));
        
        // Act
        var result = await _underTest.FindRankedGameAsync("playerId", 1000, [], CancellationToken.None);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _multiplayerService.Verify(x => x.CreateGameSearchAsync("playerId", 1000, It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}