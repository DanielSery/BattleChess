using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Miner : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Miner;

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
        int actionsCount = 0;

        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (targetFigure.IsWalkable())
            {
                actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
            }
        }

        foreach (var relative in PositionsGroups.RookDirections)
        {
            for (var targetPosition = sourcePosition + relative; board.TryGetFigure(targetPosition, out var targetFigure); targetPosition += relative)
            {
                if (targetFigure.IsWalkable())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
                }
                else
                {
                    break;
                }
            }
        }
        
        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
    {
        var move = action.TargetPosition - action.SourcePosition;
        var absX = Math.Abs(move.X);
        if (absX != 0)
        {
            return Constants.MoveImpact + absX * Constants.BuildImpact;
        }
        
        var absY = Math.Abs(move.Y);
        if (absY != 0)
        {
            return Constants.MoveImpact + absY * Constants.BuildImpact;
        }

        return 0;
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
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
                board.CreateFigure(position, new Figure(PlayerColor.Neutral, false, FigureId.Trench), onEvent);
            }
        }
    }
}