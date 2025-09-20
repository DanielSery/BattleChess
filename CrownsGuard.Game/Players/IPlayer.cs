using CrownsGuard.Game.Timers;

namespace CrownsGuard.Game.Players;

public interface IPlayer
{
    PlayerColor PlayerColor { get; }

    IPlayerTimer Timer { get; }

    string Name { get; }

    void SetTimer(IPlayerTimer timer);

    void StartTurn();

    void EndTurn(TimeSpan? forcedTurnDuration = null);
}