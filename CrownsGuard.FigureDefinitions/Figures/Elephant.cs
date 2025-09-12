using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Elephant : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Elephant;

    private static readonly Position[] Directions =
    [
        new(0, 1), new(0, -1)
    ];

    public static ArrayPoolMemory<FigureAction> GetPossibleActions(Position sourcePosition, Figure sourceFigure, Span<Figure> board)
    {
        var actionsMemory = ArrayPoolHelper.Rent<FigureAction>(36);
        var actions = actionsMemory.Span;
        int actionsCount = 0;
        
        foreach (var relative in Directions)
        {
            var isAttack = false;
            var targetPosition = sourcePosition;
            for (var i = 1; i <= 3; i++)
            {
                targetPosition += relative;
                if (!board.TryGetFigure(targetPosition, out var targetFigure))
                {
                    continue;
                }

                if (!isAttack && targetFigure.IsWalkable())
                {
                    actions[actionsCount++] = new FigureAction(FigureActionType.Move, sourcePosition, targetPosition);
                }
                else
                {
                    isAttack = true;
                    actions[actionsCount++] = new FigureAction(FigureActionType.Attack, sourcePosition, targetPosition);
                }
            }
        }
        
        return actionsMemory.WithCount(actionsCount);;
    }

    public static int EvaluateAction(Span<Figure> board, FigureAction action)
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
            else if (move.X is <= 3 and >= -3 &&
                     move.Y is <= 3 and >= -3)
            {
                var targetUnitValue = board[action.TargetPosition.GetIndex()].FigureType.GetFigureValue();
                return Constants.MeeleeAttackCoeff * targetUnitValue + Constants.PossibleMeeleeAttackImpact * 2;
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
        var targetPosition = action.TargetPosition;
        var move = action.TargetPosition - action.SourcePosition;

        if (move.X is <= 1 and >= -1 &&
            move.Y is <= 1 and >= -1)
        {
            if (board[targetPosition.GetIndex()].IsWalkable())
                board.MoveFigure(action.SourcePosition, action.TargetPosition, onEvent);
            else board.KillWithMove(action.SourcePosition, action.TargetPosition, onEvent);
        }
        else if (move.X is <= 2 and >= -2 &&
                 move.Y is <= 2 and >= -2)
        {
            var smallMove = new Position((sbyte)Math.Sign(move.X), (sbyte)Math.Sign(move.Y));
            
            var step1Position = action.SourcePosition + smallMove;
            if (board[step1Position.GetIndex()].IsWalkable())
                board.MoveFigure(action.SourcePosition, step1Position, onEvent);
            else board.KillWithMove(action.SourcePosition, step1Position, onEvent);

            if (board[step1Position.GetIndex()].FigureType != FigureId.Elephant)
                return;
           
            if (board[targetPosition.GetIndex()].IsWalkable())
                board.MoveFigure(step1Position, action.TargetPosition, onEvent);
            else board.KillWithMove(step1Position, action.TargetPosition, onEvent);
        }
        else
        {
            var smallMove = new Position((sbyte)Math.Sign(move.X), (sbyte)Math.Sign(move.Y));
            
            var step1Position = action.SourcePosition + smallMove;
            if (board[step1Position.GetIndex()].IsWalkable())
                board.MoveFigure(action.SourcePosition, step1Position, onEvent);
            else board.KillWithMove(action.SourcePosition, step1Position, onEvent);
            
            if (board[step1Position.GetIndex()].FigureType != FigureId.Elephant)
                return;
            
            var step2Position = action.SourcePosition + smallMove + smallMove;
            if (board[step2Position.GetIndex()].IsWalkable())
                board.MoveFigure(step1Position, step2Position, onEvent);
            else board.KillWithMove(step1Position, step2Position, onEvent);
            
            if (board[step2Position.GetIndex()].FigureType != FigureId.Elephant)
                return;
            
            if (board[targetPosition.GetIndex()].IsWalkable())
                board.MoveFigure(step2Position, action.TargetPosition, onEvent);
            else board.KillWithMove(step2Position, action.TargetPosition, onEvent);
        }
    }
}