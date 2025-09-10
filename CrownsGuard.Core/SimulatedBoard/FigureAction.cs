using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Core.SimulatedBoard;

public readonly struct FigureAction
{
    public static readonly FigureAction None = new FigureAction();

    public readonly FigureActionType FigureActionType;
    public readonly Position SourcePosition;
    public readonly Position TargetPosition;

    public FigureAction(FigureActionType figureActionType, Position sourcePosition, Position targetPosition)
    {
        FigureActionType = figureActionType;
        SourcePosition = sourcePosition;
        TargetPosition = targetPosition;
    }
}