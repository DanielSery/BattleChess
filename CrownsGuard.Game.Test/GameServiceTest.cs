using AwesomeAssertions;
using CrownsGuard.Core.Figures;
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
        var player1Mock = CreatePlayer<IPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.Player.Should().Be(Player.White);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Never);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.Player.Should().Be(Player.Black);
        player1Mock.Verify(x => x.StartTurn(), Times.Never);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.Player.Should().Be(Player.Black);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.Player.Should().Be(Player.White);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void WhenSurrender_2HumanPlayers_CurrentPlayerLoses()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        Player losingPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.Surrender();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        losingPlayer.Should().Be(Player.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            losingPlayer = e.Lost!.Player;
        }
    }

    [Fact]
    public void WhenSurrender_1HumanPlayer_WhitePlayerLoses()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IPlayerInfo>(Player.Black, true);

        Player losingPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.Surrender();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        losingPlayer.Should().Be(Player.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            losingPlayer = e.Lost!.Player;
        }
    }

    [Fact]
    public void WhenWhiteWins_WhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.PlayerWin(player1Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenBlackWins_BlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.PlayerWin(player2Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenWhiteLoses_BlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.PlayerLost(player1Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenBlackLoses_WhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.PlayerLost(player2Mock.Object, WinType.OutOfTime, true);
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenGameIsOver_IgnoreNextTurn()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
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
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);
        gameService.EndTurn();

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenWhiteLosesKing_OnWhiteNewTurnBlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.EndTurn();

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenBlackLosesKing_OnBlackNewTurnWhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);
        gameService.EndTurn();

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenBlackLosesKing_OnWhiteNewTurnWhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.EndTurn();

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenBothLoseKing_OnBlackNewTurnWhiteWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);
        gameService.EndTurn();

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.White);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    [Fact]
    public void WhenBothLoseKing_OnWhiteNewTurnBlackWins()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IControlledPlayerInfo>(Player.White, true);
        var player2Mock = CreatePlayer<IControlledPlayerInfo>(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.EndTurn();

        Player winningPlayer = Player.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        player1Mock.Setup(x => x.Figures).Returns([]);
        player2Mock.Setup(x => x.Figures).Returns([]);
        gameService.StartTurn();
        gameService.PlayerWon -= GameServiceOnPlayerWon;

        winningPlayer.Should().Be(Player.Black);
        gameService.GameRunning.Should().BeFalse();
        return;

        void GameServiceOnPlayerWon(object? sender, WinResult e)
        {
            winningPlayer = e.Won!.Player;
        }
    }

    private static Mock<T> CreatePlayer<T>(Player player, bool hasKing)
        where T : class, IPlayerInfo
    {
        var playerInfo = new Mock<T>();
        playerInfo.SetupGet(x => x.Player).Returns(player);
        if (hasKing)
        {
            playerInfo.Setup(x => x.Figures).Returns([new Figure(playerInfo.Object, NoneFigureType.Instance, true)]);
        }
        else
        {
            playerInfo.Setup(x => x.Figures).Returns([]);
        }

        return playerInfo;
    }
}