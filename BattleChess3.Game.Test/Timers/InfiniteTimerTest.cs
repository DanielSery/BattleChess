using BattleChess3.Game.Timers;

namespace BattleChess3.Game.Test.Timers;

public class InfiniteTimerTest
{
    [Fact]
    public void WhenInTurn_TimeNotDecreases()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(null);

        Assert.Equal(timeBefore, chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }

    [Fact]
    public void WhenInTurn_ForcedTimeDiffIsIgnored()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(10));

        Assert.Equal(timeBefore, chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }

    [Fact]
    public void WhenNotInTurn_TimeNotDecreases()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        Thread.Sleep(50);

        Assert.Equal(timeBefore, chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }

    [Fact]
    public void AfterStart_TimeIsNotChaged()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(0));

        Assert.Equal(timeBefore, chessTimer.RemainingTime);
        Assert.Equal(chessTimer.LastTurnElapsedTime, TimeSpan.Zero);
    }
}