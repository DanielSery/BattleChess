using AwesomeAssertions;
using CrownsGuard.Core.Figures;
using CrownsGuard.Game.Players;
using Moq;

namespace CrownsGuard.Game.Test;

public class GameServiceTest
{
    [Fact]
    public void StartGame_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White, GetBoard(true, true));

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.PlayerColor.Should().Be(PlayerColor.White);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Never);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        gameService.CurrentPlayerInfo.PlayerColor.Should().Be(PlayerColor.Black);
        player1Mock.Verify(x => x.StartTurn(), Times.Never);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer<IPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        PlayerColor losingPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IPlayer>(PlayerColor.Black);

        PlayerColor losingPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White, GetBoard(true, true));
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        ChangeBoard(gameService.Board, false, true);
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        ChangeBoard(gameService.Board, false, true);
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White, GetBoard(true, true));
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        ChangeBoard(gameService.Board, true, false);
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        ChangeBoard(gameService.Board, true, false);
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.White, GetBoard(true, true));
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        ChangeBoard(gameService.Board, false, false);
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
        var player1Mock = CreatePlayer<IControlledPlayer>(PlayerColor.White);
        var player2Mock = CreatePlayer<IControlledPlayer>(PlayerColor.Black);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, PlayerColor.Black, GetBoard(true, true));
        gameService.EndTurn();

        PlayerColor winningPlayerColor = PlayerColor.Neutral;
        gameService.PlayerWon += GameServiceOnPlayerWon;
        ChangeBoard(gameService.Board, false, false);
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

    private static void ChangeBoard(Figure[] board, bool whiteKing, bool blackKing)
    {
        if (whiteKing)
        {
            board[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        }
        else
        {
            board[0] = Figure.King | Figure.IsWhite;
        }

        if (blackKing)
        {
            board[63] = Figure.King | Figure.IsBlack | Figure.IsKing;
        }
        else
        {
            board[63] = Figure.King | Figure.IsBlack;
        }
    }

    private static Figure[] GetBoard(bool whiteKing, bool blackKing)
    {
        var figures = Enumerable.Repeat(Figure.Empty, 64).ToArray();

        if (whiteKing)
        {
            figures[0] = Figure.King | Figure.IsWhite | Figure.IsKing;
        }

        if (blackKing)
        {
            figures[63] = Figure.King | Figure.IsBlack | Figure.IsKing;
        }

        return figures;
    }

    private static Mock<T> CreatePlayer<T>(PlayerColor playerColor)
        where T : class, IPlayer
    {
        var playerInfo = new Mock<T>();
        playerInfo.SetupGet(x => x.PlayerColor).Returns(playerColor);
        return playerInfo;
    }
}