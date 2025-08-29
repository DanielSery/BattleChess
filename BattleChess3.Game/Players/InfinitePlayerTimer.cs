// Copyright (c) Veeam Software Group GmbH

namespace BattleChess3.Game.Players;

public class InfinitePlayerTimer : IPlayerTimer
{
    public static readonly InfinitePlayerTimer Instance = new InfinitePlayerTimer();

    /// <inheritdoc />
    public TimeSpan RemainingTime => TimeSpan.MaxValue;

    /// <inheritdoc />
    public void StartTurnTimer()
    {
    }

    /// <inheritdoc />
    public TimeSpan EndTurnTimer(TimeSpan? forcedTime)
    {
        return TimeSpan.Zero;
    }
}