using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Core.Figures;

public readonly struct FigureAction
{
    public static readonly FigureAction None = new FigureAction();

    public readonly FigureActionType FigureActionType;
    public readonly Position SourcePosition;
    public readonly Position TargetPosition;
    public readonly Figure SourceFigure;
    public readonly Figure TargetFigure;

    public FigureAction(FigureActionType figureActionType, Position sourcePosition, Position targetPosition, Figure sourceFigure, Figure targetFigure)
    {
        FigureActionType = figureActionType;
        SourcePosition = sourcePosition;
        TargetPosition = targetPosition;
        SourceFigure = sourceFigure;
        TargetFigure = targetFigure;
    }
}