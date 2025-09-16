using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Cannon : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Cannon;

    private static readonly Position[] BlackAttackPositions =
    [
        new(0, 2), new(0, 3), new(0, 4),
    ];

    private static readonly Position[] WhiteAttackPositions =
    [
        new(0, -2), new(0, -3), new(0, -4),
    ];

    public static void GetPossibleActions(int sourceIndex, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
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
        foreach (Position relative in attackPositions)
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