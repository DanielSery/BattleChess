using AwesomeAssertions;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.Game.Test.Timers;

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

        chessTimer.RemainingTime.Should().Be(timeBefore - chessTimer.LastTurnElapsedTime);
        chessTimer.LastTurnElapsedTime.Should().NotBe(TimeSpan.Zero);
    }

    [Fact]
    public void WhenInTurn_CanForceTimeDiff()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(0));
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(10));

        chessTimer.RemainingTime.Should().Be(timeBefore - TimeSpan.FromSeconds(10));
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void WhenNotInTurn_TimeNotDecreases()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(0));
        var timeBefore = chessTimer.RemainingTime;

        Thread.Sleep(50);

        chessTimer.RemainingTime.Should().Be(timeBefore);
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void AfterStart_TimeIsAdded()
    {
        var chessTimer = new ChessTimer(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));
        var timeBefore = chessTimer.RemainingTime;

        chessTimer.StartTurnTimer();
        Thread.Sleep(50);
        chessTimer.EndTurnTimer(TimeSpan.FromSeconds(0));

        chessTimer.RemainingTime.Should().Be(timeBefore + TimeSpan.FromSeconds(10));
        chessTimer.LastTurnElapsedTime.Should().Be(TimeSpan.Zero);
    }
}