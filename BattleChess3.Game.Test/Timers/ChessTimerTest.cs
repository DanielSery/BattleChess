using BattleChess3.Game.Timers;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.Timers;

public class ChessTimerTest
{
    [Fact]
    public void WhenInTurn_TimeDecreases()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(0));
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(null);

        Assert.Equal(timeBefore - chessTimer.LastTurnElapsedTime, chessTimer.RemainingTime);
        Assert.NotEqual(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }

    [Fact]
    public void WhenInTurn_CanForceTimeDiff()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(0));
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(10));

        Assert.Equal(timeBefore - TimeSpan.FromSeconds(10), chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void WhenNotInTurn_TimeNotDecreases()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(0));
        var timeBefore = chessTimer.RemainingTime;

        Thread.Sleep(50);

        Assert.Equal(timeBefore, chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }

    [Fact]
    public void AfterStart_TimeIsAdded()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(0));

        Assert.Equal(timeBefore + TimeSpan.FromSeconds(10), chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }
}