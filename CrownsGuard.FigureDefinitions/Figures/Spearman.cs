using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Spearman : ICrownsGuardFigureTypeInfo
{
    public Figure Figure => Figure.Spearman;

    private static readonly Position[] AttackPositions =
    [
        new(-1, 1), new(1, 1), new(-1, -1), new(1, -1)
    ];

    private static readonly Position[] BlackMovePositions =
    [
        new(-1, 0), new(1, 0), new(0, 1)
    ];

    private static readonly Position[] WhiteMovePositions =
    [
        new(-1, 0), new(1, 0), new(0, -1)
    ];

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        var movePositions = sourceFigure.IsBlack() ? BlackMovePositions : WhiteMovePositions;
        foreach (var relative in movePositions)
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

        foreach (var relative in AttackPositions)
        {
            var targetPosition = sourcePosition + relative;
            if (!board.TryGetFigure(targetPosition, out var targetFigure))
            {
                continue;
            }

            if (sourceFigure.CanAttack(targetFigure))
            {
                actions.Push(new FigureAction(FigureActionType.MeeleeAttack, sourcePosition, targetPosition));
            }
            else
            {
                actions.Push(new FigureAction(FigureActionType.PossibleMeeleeAttack, sourcePosition, targetPosition));
            }
        }
    }
}