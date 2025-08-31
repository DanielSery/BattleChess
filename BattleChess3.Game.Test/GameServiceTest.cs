using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;
using BattleChess3.Game.Players;
using BattleChess3.Game.Timers;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test;

public class GameServiceTest
{
    [Fact]
    public void StartGame_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var (player1, timer1Mock) = CreatePlayer(Player.White, true);
        var (player2, timer2Mock) = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.White);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        timer1Mock.Verify(x => x.StartTurnTimer(), Times.Once);
        timer2Mock.Verify(x => x.StartTurnTimer(), Times.Never);
    }

    [Fact]
    public void StartGame_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var (player1, timer1Mock) = CreatePlayer(Player.White, true);
        var (player2, timer2Mock) = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.Black);

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        timer1Mock.Verify(x => x.StartTurnTimer(), Times.Never);
        timer2Mock.Verify(x => x.StartTurnTimer(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleWhite()
    {
        var gameService = new GameService();
        var (player1, timer1Mock) = CreatePlayer(Player.White, true);
        var (player2, timer2Mock) = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.White);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        timer1Mock.Verify(x => x.StartTurnTimer(), Times.Once);
        timer2Mock.Verify(x => x.StartTurnTimer(), Times.Once);
    }

    [Fact]
    public void StartTurn_SetsCorrect_ForSingleBlack()
    {
        var gameService = new GameService();
        var (player1, timer1Mock) = CreatePlayer(Player.White, true);
        var (player2, timer2Mock) = CreatePlayer(Player.Black, true);

        gameService.StartGame(player1, player2, Player.Black);
        gameService.EndTurn();
        gameService.StartTurn();

        Assert.True(gameService.GameRunning, "gameService.GameRunning");
        Assert.Equal(Player.White, gameService.CurrentPlayerInfo.Player);
        timer1Mock.Verify(x => x.StartTurnTimer(), Times.Once);
        timer2Mock.Verify(x => x.StartTurnTimer(), Times.Once);
    }

    private static (ControlledPlayerInfo, Mock<IPlayerTimer>) CreatePlayer(Player player, bool hasKing)
    {
        var timerMock = new Mock<IPlayerTimer>();
        var playerInfo = new ControlledPlayerInfo(player, string.Empty);
        playerInfo.SetTimer(timerMock.Object);
        if (hasKing)
        {
            playerInfo.Figures.Add(new Figure(playerInfo, NoneFigureType.Instance, true));
        }
        
        return (playerInfo, timerMock);
    }
}