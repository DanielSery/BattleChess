using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Cannon
{
    private static readonly short[] BlackAttackPositions =
    [
        unchecked((byte)+0)+2*PositionConstants.YOffset, 
        unchecked((byte)+0)+3*PositionConstants.YOffset, 
        unchecked((byte)+0)+4*PositionConstants.YOffset,
    ];

    private static readonly short[] WhiteAttackPositions =
    [
        unchecked((byte)+0)-2*PositionConstants.YOffset, 
        unchecked((byte)+0)-3*PositionConstants.YOffset, 
        unchecked((byte)+0)-4*PositionConstants.YOffset,
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionConstants.QueenDirections)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return;
            }
        }
        
        var attackPositions = sourceFigure.IsBlack() ? BlackAttackPositions : WhiteAttackPositions;
        foreach (var relative in attackPositions)
        {
            var targetIndex = sourceIndex.GetWithOffset(relative);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];
            
            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.CannonAttack, sourceIndex, targetIndex, sourceFigure));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleCannonAttack, sourceIndex, targetIndex, sourceFigure));
            }
        }
    }

    public static void ExecuteAttack(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var attackPositions = action.SourceFigure.IsBlack() ? BlackAttackPositions : WhiteAttackPositions;
        foreach (var attackPosition in attackPositions)
        {
            var targetIndex = action.SourceIndex.GetWithOffset(attackPosition);
            if (targetIndex == -1) continue;
            var targetFigure = board[targetIndex];

            if (!targetFigure.IsEmpty())
            {
                board.KillWithoutMove(action.SourceIndex, (byte)targetIndex, onEvent);
            }
        }
    }
}