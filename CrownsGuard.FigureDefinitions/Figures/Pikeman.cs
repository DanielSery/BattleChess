using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Pikeman : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Pikeman;

    private static readonly Position[] BlackAttackPositions =
    [
        new(-1, 1), new(1, 1)
    ];

    private static readonly Position[] WhiteAttackPositions =
    [
        new(-1, 1), new(1, 1)
    ];

    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    public static void GetPossibleActions(Position sourcePosition, Figure sourceFigure, ReadOnlySpan<Figure> board, Stack<FigureAction> actions)
    {
        foreach (var relative in MovePositions)
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

        var attackPositions = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackPositions : WhiteAttackPositions;
        foreach (var relative in attackPositions)
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
