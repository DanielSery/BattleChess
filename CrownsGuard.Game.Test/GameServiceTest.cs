using AwesomeAssertions;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Players;
using Moq;

namespace CrownsGuard.Game.Test;

public class GameServiceTest
{
    [Fact]
    public void StartGame_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.PlayerColor.Should().Be(PlayerColor.White);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Never);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.PlayerColor.Should().Be(PlayerColor.Black);
        player1Mock.Verify(x => x.StartTurn(), Times.Never);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.PlayerColor.Should().Be(PlayerColor.Black);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.PlayerColor.Should().Be(PlayerColor.White);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void WhenSurrender_2HumanPlayers_CurrentPlayerLoses()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        PlayerColor losingPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.Surrender();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        losingPlayerColor.Should().Be(PlayerColor.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            losingPlayerColor = e.Lost!.PlayerColor;
        }
    }

    [Fact]
    public void WhenSurrender_1HumanPlayer_WhitePlayerLoses()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(PlayerColor.Black, true);

        PlayerColor losingPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.Surrender();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        losingPlayerColor.Should().Be(PlayerColor.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            losingPlayerColor = e.Lost!.PlayerColor;
        }
    }

    [Fact]
    public void WhenWhiteWins_WhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.PlayerWin(player1Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenBlackWins_BlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.PlayerWin(player2Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenWhiteLoses_BlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.PlayerLost(player1Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenBlackLoses_WhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.PlayerLost(player2Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenGameIsOver_IgnoreNextTurn()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        var currentPlayer = gameService.CurrentPlayerInfo;

        gameService.PlayerLost(player2Mock.Object, WinType.OutOfTime, true);
        gameService.CurrentPlayerInfo.Should().Be(currentPlayer);

        gameService.StartTurn();
        gameService.CurrentPlayerInfo.Should().Be(currentPlayer);
    }

    [Fact]
    public void WhenWhiteLosesKing_OnBlackNewTurnBlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White);
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenWhiteLosesKing_OnWhiteNewTurnBlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenBlackLosesKing_OnBlackNewTurnWhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White);
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenBlackLosesKing_OnWhiteNewTurnWhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenBothLoseKing_OnBlackNewTurnWhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White);
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    [Fact]
    public void WhenBothLoseKing_OnWhiteNewTurnBlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(PlayerColor.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black);
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayerColor.Should().Be(PlayerColor.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayerColor = e.Won!.PlayerColor;
        }
    }

    private static Mock<T> CreatePlayer<T>(PlayerColor playerColor, bool hasKing)
        where T : class, IPlayerInfo
    {
        var playerInfo = new Mock<T>();
        playerInfo.SetupGet(x => x.PlayerColor).Returns(playerColor);
        if (hasKing)
        {
            playerInfo.Setup(x => x.Figures).Returns([new FigureWithInfo(playerInfo.Object, NoneFigureTypeInfo.Instance, true)]);
        }
        else
        {
            playerInfo.Setup(x => x.Figures).Returns([]);
        }

        return playerInfo;
    }
}