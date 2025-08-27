using BattleChess3.Game.Players;
using Xunit;
using Assert = Xunit.Assert;

namespace BattleChess3.Game.Test.Players;

public class PlayerInfoTests
{
    [Fact]
    public void WhenInTurn_TimeDecreases()
    {
        var playerInfo = new PlayerInfo(Player.White, string.Empty, null, null);
        var timeBefore = playerInfo.RemainingTime;

        playerInfo.StartTurn();
        Thread.Sleep(50);
        var turnDuration = playerInfo.OnEndingTurn(null);

        Assert.Equal(timeBefore - turnDuration, playerInfo.RemainingTime);
        Assert.NotEqual(turnDuration,  TimeSpan.Zero);
    }

    [Fact]
    public void WhenInTurn_CanForceTimeDiff()
    {
        var playerInfo = new PlayerInfo(Player.White, string.Empty, null, null);
        var timeBefore = playerInfo.RemainingTime;

        playerInfo.StartTurn();
        var turnDuration = playerInfo.OnEndingTurn(TimeSpan.FromSeconds(10));

        Assert.Equal(timeBefore - TimeSpan.FromSeconds(10), playerInfo.RemainingTime);
        Assert.Equal(TimeSpan.FromSeconds(10), turnDuration);
    }

    [Fact]
    public void WhenNotInTurn_TimeNotDecreases()
    {
        var playerInfo = new PlayerInfo(Player.White, string.Empty, null, null);
        var timeBefore = playerInfo.RemainingTime;

        Thread.Sleep(50);

        Assert.Equal(timeBefore, playerInfo.RemainingTime);
    }
}