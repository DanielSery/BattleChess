using AwesomeAssertions;
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

        chessTimer.RemainingTime.Should().Be(timeBefore);
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void WhenInTurn_ForcedTimeDiffIsIgnored()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(10));

        chessTimer.RemainingTime.Should().Be(timeBefore);
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void WhenNotInTurn_TimeNotDecreases()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        Thread.Sleep(50);

        chessTimer.RemainingTime.Should().Be(timeBefore);
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void AfterStart_TimeIsNotChanged()
    {
        var chessTimer = InfinitePlayerTimer.Instance;
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(0));

        chessTimer.RemainingTime.Should().Be(timeBefore);
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.Zero);
    }
}