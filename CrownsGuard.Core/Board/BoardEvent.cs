using CrownsGuard.Core.Figures;

namespace CrownsGuard.Core.Board;

public readonly struct BoardEvent
{
    public BoardEventType EventType { get; }
    public byte SourceIndex { get; }
    public byte TargetIndex { get; }
    public Figure TargetedFigure { get; }

    public BoardEvent(BoardEventType type, byte sourceIndex, byte targetIndex, Figure targetedFigure)
    {
        EventType = type;
        SourceIndex = sourceIndex;
        TargetIndex = targetIndex;
        TargetedFigure = targetedFigure;
    }
}