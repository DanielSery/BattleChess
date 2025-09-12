using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Nordguard: ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Nordguard;

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.BishopDirections)
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
        
        foreach (var relative in PositionsGroups.RookDirections)
        {
            var targetPosition = sourcePosition;
            for (var i = 1; i <= 3; i++)
            {
                targetPosition += relative;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    break;
                }

                if (sourceFigure.CanAttack(targetFigure))
                {
                    actions.Push(new FigureAction(FigureActionType.Move, sourcePosition, targetPosition));
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleAttack, sourcePosition, targetPosition));
                }
                else
                {
                    break;
                }
            }
        }
        
        return;;
    }

    public static int EvaluateAction(ReadOnlySpan<Figure> board, FigureAction action)
    {
        if (action.FigureActionType == FigureActionType.Attack)
        {
            var move = action.TargetPosition - action.SourcePosition;
            if (move.X is <= 1 and >= -1 &&
                move.Y is <= 1 and >= -1)
            {
                var targetUnitValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
                return Constants.MeeleeAttackCoeff * targetUnitValue;
            }
            else if (move.X is <= 2 and >= -2 &&
                     move.Y is <= 2 and >= -2)
            {
                var targetUnitValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
                return Constants.MeeleeAttackCoeff * targetUnitValue + Constants.PossibleMeeleeAttackImpact;
            }
        }
        else if (action.FigureActionType == FigureActionType.Move)
        {
            return Constants.MoveImpact;
        }

        return 0;
    }

    public static void ExecuteAction(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var move = action.TargetPosition - action.SourcePosition;

        if (move.X is <= 1 and >= -1 &&
            move.Y is <= 1 and >= -1)
        {
            board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (move.X is <= 2 and >= -2 &&
                 move.Y is <= 2 and >= -2)
        {
            var smallMove = new Position((sbyte)Math.Sign(move.X), (sbyte)Math.Sign(move.Y));
            var sourcePosition = action.SourcePosition;
            
            board.KillWithMove(sourcePosition, sourcePosition + smallMove, onEvent);
            var figure = board[(sourcePosition + smallMove).GetIndex()];
            if (figure.FigureType != FigureId.Blade)
                return;
           
            board.KillWithMove(sourcePosition + smallMove, action.TargetPosition, onEvent);
        }
    }
}