using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Musketeer : ICrownsGuardFigureTypeInfo
{
    public FigureId FigureId => FigureId.Musketeer;

    private static readonly Position[] BlackAttackDirections =
    [
        new(-1, 1), new(0, 1), new(1, 1)
    ];

    private static readonly Position[] WhiteAttackDirections =
    [
        new(-1, -1), new(0, -1), new(1, -1)
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

        var attackDirections = sourceFigure.PlayerColor == PlayerColor.Black ? BlackAttackDirections : WhiteAttackDirections;
        foreach (var relative in attackDirections)
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
                    actions.Push(new FigureAction(FigureActionType.RangedAttack, sourcePosition, targetPosition));
                    break;
                }
                else if (targetFigure.IsWalkable())
                {
                    actions.Push(new FigureAction(FigureActionType.PossibleRangedAttack, sourcePosition, targetPosition));
                }
                else
                {
                    break;
                }
            }
        }
    }
}