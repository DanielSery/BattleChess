namespace CrownsGuard.Core.Players;

public class NeutralPlayer : IPlayer
{
    public static readonly NeutralPlayer Instance = new NeutralPlayer();

    private NeutralPlayer() { }

    /// <inheritdoc />
    public PlayerColor PlayerColor => PlayerColor.Neutral;
}