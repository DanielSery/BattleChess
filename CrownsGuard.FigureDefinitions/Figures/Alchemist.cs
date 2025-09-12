
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Alchemist : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Alchemist;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out Figure targetFigure))
            {
                continue;
            }
            
            if (targetFigure.IsWalkable() || targetFigure.FigureType == FigureId.Explosives)
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
            }
        }
    }

    public static int EvaluateAction(FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact + Constants.BuildImpact;
        }
        else
        {
            return 0;
        }
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        if (action.FigureActionType == FigureActionType.Move)
        {
            board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
            CreateExplosive(action.SourcePosition, action.TargetPosition - action.SourcePosition, board, onEvent);
        }
        else
        {
            throw new NotSupportedException($"Invalid action type {action.FigureActionType}");
        }
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
            board.CreateFigure(targetPosition, new Figure(sourceFigure.PlayerColor, false, FigureId.Explosives), onEvent);
        }
    }
}