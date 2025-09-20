using CrownsGuard.Game.Timers;

namespace CrownsGuard.Game.Players;

public class NeutralPlayer : IPlayer
{
    public static readonly NeutralPlayer Instance = new NeutralPlayer();

    private NeutralPlayer() { }

    /// <inheritdoc />
    public PlayerColor PlayerColor => PlayerColor.Neutral;

    public IPlayerTimer Timer => InfinitePlayerTimer.Instance;

    public string Name => "Neutral";

    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
        throw new NotSupportedException();
    }

    public void SetTimer(IPlayerTimer timer)
    {
        throw new NotSupportedException();
    }

    public void StartTurn()
    {
        throw new NotSupportedException();
    }
}