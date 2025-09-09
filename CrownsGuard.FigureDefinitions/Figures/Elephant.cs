using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Elephant : ICrownsGuardFigureType
{
    public int FigureValue => 10;

    private static readonly Position[] Directions =
    [
        new(0, 1), new(0, -1)
    ];

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        foreach (var relative in Directions)
        {
            var isAttack = false;
            for (var i = 1; i <= 3; i++)
            {
                var targetPosition = sourcePosition + relative * i;
                if (!board.TryGetFigure(targetPosition, out var targetFigure)) continue;

                if (!isAttack && targetFigure.IsWalkable())
                {
                    actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Move, sourcePosition, targetPosition);
                }
                else
                {
                    isAttack = true;
                    actions[actionsCount++] = new FigureAction(sourceFigure.FigureType, FigureActionType.Attack, sourcePosition, targetPosition);
                }
            }
        }
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
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
            var smallMove = new Position((short)Math.Sign(move.X), (short)Math.Sign(move.Y));
            
            var step1Position = action.SourcePosition + smallMove;
            if (board[step1Position.GetIndex()].IsWalkable())
                board.MoveFigure(action.SourcePosition, step1Position, onEvent);
            else board.KillWithMove(action.SourcePosition, step1Position, onEvent);

            if (board[step1Position.GetIndex()].FigureType != FigureType.Elephant)
                return;
           
            if (board[targetPosition.GetIndex()].IsWalkable())
                board.MoveFigure(step1Position, action.TargetPosition, onEvent);
            else board.KillWithMove(step1Position, action.TargetPosition, onEvent);
        }
        else
        {
            var smallMove = new Position((short)Math.Sign(move.X), (short)Math.Sign(move.Y));
            
            var step1Position = action.SourcePosition + smallMove;
            if (board[step1Position.GetIndex()].IsWalkable())
                board.MoveFigure(action.SourcePosition, step1Position, onEvent);
            else board.KillWithMove(action.SourcePosition, step1Position, onEvent);
            
            if (board[step1Position.GetIndex()].FigureType != FigureType.Elephant)
                return;
            
            var step2Position = action.SourcePosition + smallMove * 2;
            if (board[step2Position.GetIndex()].IsWalkable())
                board.MoveFigure(step1Position, step2Position, onEvent);
            else board.KillWithMove(step1Position, step2Position, onEvent);
            
            if (board[step2Position.GetIndex()].FigureType != FigureType.Elephant)
                return;
            
            if (board[targetPosition.GetIndex()].IsWalkable())
                board.MoveFigure(step2Position, action.TargetPosition, onEvent);
            else board.KillWithMove(step2Position, action.TargetPosition, onEvent);
        }
    }
}