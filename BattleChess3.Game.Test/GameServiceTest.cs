using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;
using BattleChess3.Game.Players;
using Moq;

namespace BattleChess3.Game.Test;

public class GameServiceTest
{
    [Fact]
    public void StartGame_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer(Player.White, true);
        var player2Mock = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Never);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer(Player.White, true);
        var player2Mock = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.Black, gameService.CurrentPlayerInfo.Player);
        player1Mock.Verify(x => x.StartTurn(), Times.Never);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer(Player.White, true);
        var player2Mock = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.White);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.Black, gameService.CurrentPlayerInfo.Player);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var player1Mock = CreatePlayer(Player.White, true);
        var player2Mock = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1Mock.Object, player2Mock.Object, Player.Black);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        player1Mock.Verify(x => x.StartTurn(), Times.Once);
        player2Mock.Verify(x => x.StartTurn(), Times.Once);
    }

    private static Mock<IPlayerInfo> CreatePlayer(Player player, bool hasKing)
    {
        var playerInfo = new Mock<IPlayerInfo>();
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