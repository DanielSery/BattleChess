using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Miner : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Miner;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition, sourceFigure, targetFigure));
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.MinerMove, sourcePosition, targetPosition, sourceFigure, targetFigure));
                }
                else
                {
                    break;
                }
            }
        }
    }

    public static void ExecuteMove(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
        var difference = action.TargetPosition - action.SourcePosition;
        var direction = new Position((sbyte) Math.Sign(difference.X), (sbyte) Math.Sign(difference.Y));
        for (var position = action.SourcePosition; position != action.TargetPosition; position += direction)
        {
            if (!board.TryGetFigure(position, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsEmpty())
            {
                board.CreateFigure(position, Figure.Trench, onEvent);
            }
        }
    }
}