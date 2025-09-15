using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Alchemist : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Alchemist;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }
            
            if (targetFigure.IsWalkable() || targetFigure.GetFigureType() == Figure.Explosives)
            {
                actions.Push(new FigureAction(FigureActionType.AlchemistMove, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        CreateExplosive(action.SourcePosition, action.TargetPosition - action.SourcePosition, board, onEvent);
    }

    private static void CreateExplosive(Position sourcePosition, Position move, Span<Figure> board, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var targetPosition = sourcePosition + move + move;
        if (!board.TryGetFigure(targetPosition, out var targetFigure))
        {
            return;
        }

        if (targetFigure.IsEmpty())
        {
            var sourceFigure = board[sourcePosition.GetIndex()];
            board.CreateFigure(targetPosition, Figure.Explosives | sourceFigure.GetFigureColor(), onEvent);
        }
    }
}