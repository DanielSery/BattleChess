namespace CrownsGuard.Core.GameBoard;

public readonly struct BoardEvent
{
    public BoardEventType EventType { get; }
    public Position SourcePosition { get; }
    public Position TargetPosition { get; }

    public BoardEvent(BoardEventType type, Position sourcePosition, Position targetPosition)
    {
        EventType = type;
        SourcePosition = sourcePosition;
        TargetPosition = targetPosition;
    }
}