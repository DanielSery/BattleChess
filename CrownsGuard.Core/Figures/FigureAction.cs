using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Core.Figures;

public readonly struct FigureAction
{
    public static readonly FigureAction None = new FigureAction();

    public readonly FigureActionType FigureActionType;
    public readonly byte SourceIndex;
    public readonly byte TargetIndex;
    public readonly Figure SourceFigure;

    public FigureAction(FigureActionType figureActionType, int sourceIndex, int targetIndex, Figure sourceFigure)
    {
        FigureActionType = figureActionType;
        SourceIndex = (byte)sourceIndex;
        TargetIndex = (byte)targetIndex;
        SourceFigure = sourceFigure;
    }
}