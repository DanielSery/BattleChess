namespace CrownsGuard.Core.GameBoard;

public readonly struct BoardEvent
{
    public BoardEventType EventType { get; }
    public byte SourceIndex { get; }
    public byte TargetIndex { get; }

    public BoardEvent(BoardEventType type, byte sourceIndex, byte targetIndex)
    {
        EventType = type;
        SourceIndex = sourceIndex;
        TargetIndex = targetIndex;
    }
}