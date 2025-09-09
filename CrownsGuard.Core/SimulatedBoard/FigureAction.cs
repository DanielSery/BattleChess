using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Core.SimulatedBoard;

public readonly struct FigureAction
{
    public readonly FigureType FigureType;
    public readonly FigureActionType FigureActionType;
    public readonly Position SourcePosition;
    public readonly Position TargetPosition;

    public FigureAction(FigureType figureType, FigureActionType figureActionType, Position sourcePosition, Position targetPosition)
    {
        FigureType = figureType;
        FigureActionType = figureActionType;
        SourcePosition = sourcePosition;
        TargetPosition = targetPosition;
    }
}