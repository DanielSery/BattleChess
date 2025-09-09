using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Core.SimulatedBoard;

public readonly struct BoardEvent
{
    public BoardEventType EventType { get; }
    public Position Position { get; }

    public BoardEvent(BoardEventType type, Position position)
    {
        EventType = type;
        Position = position;
    }
}