using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
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

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in PositionsGroups.QueenDirections)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                return;
            }
        }
        
        var attackPositions = sourceFigure.IsBlack() ? BlackAttackPositions : WhiteAttackPositions;
        foreach (Position attackPosition in attackPositions)
        {
            var targetPosition = sourcePosition + attackPosition;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }
            
            if (sourceFigure.IsEnemyTo(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.CannonAttack, sourcePosition, targetPosition));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleCannonAttack, sourcePosition, targetPosition));
            }
        }
    }

    public static void ExecuteAttack(Span<Figure> board, FigureAction action, Action<BoardEvent, Span<Figure>> onEvent)
    {
        var sourceFigure = board[action.SourcePosition.GetIndex()];
        var attackPositions = sourceFigure.IsBlack() ? BlackAttackPositions : WhiteAttackPositions;

        foreach (var attackPosition in attackPositions)
        {
            var targetPosition = action.SourcePosition + attackPosition;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (!targetFigure.IsEmpty())
            {
                board.KillWithoutMove(action.SourcePosition, targetPosition, onEvent);
            }
        }
    }
}