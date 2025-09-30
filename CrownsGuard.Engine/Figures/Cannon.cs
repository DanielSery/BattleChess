using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Figures;

public static class Cannon
{
    private static readonly short[] BlackAttackPositions =
    [
        PositionConstants.U2,
        PositionConstants.U3,
        PositionConstants.U4,
    ];

    private static readonly short[] WhiteAttackPositions =
    [
        PositionConstants.D2,
        PositionConstants.D3,
        PositionConstants.D4,
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