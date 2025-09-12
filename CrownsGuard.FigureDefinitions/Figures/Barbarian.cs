using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Barbarian : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Barbarian;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.KnightPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
        
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var movedPosition = sourcePosition + relative;
            if (!board.TryGetFigure(movedPosition, out var movedFigure) ||
                movedFigure.IsEmpty())
            {
                continue;
            }

            for (var targetPosition = sourcePosition + relative + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (targetFigure.IsEmpty())
                {
                    actions.Push(new FigureAction(FigureActionType.Special, movedPosition, targetPosition));
                }
                else
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleSpecial, movedPosition, targetPosition));
                }
            }
        }
        
        return;
    }

    public static int EvaluateAction()
    {
        return Constants.MoveImpact;
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType is FigureActionType.Move or FigureActionType.Special)
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        else
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
    }
}