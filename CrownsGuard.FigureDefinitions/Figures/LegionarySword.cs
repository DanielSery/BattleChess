using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class LegionarySword : ICrownsGuardFigureType
{
    public int FigureValue => 2;

    public int FigureId => (int)CrownsGuardFigureIds.LegionarySwordId;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (TryGetAttackAction(unitTile, board, new Position(1, 1), out var attackAction1))
        {
            yield return attackAction1;
        }

        if (TryGetAttackAction(unitTile, board, new Position(-1, 1), out var attackAction2))
        {
            yield return attackAction2;
        }

        if (TryGetMoveAction(unitTile, board, new Position(0, 1), out var moveAction1))
        {
            yield return moveAction1;
        }
        else
        {
            yield break;
        }

        if (unitTile.RelativePosition.Y == 1 &&
            TryGetMoveAction(unitTile, board, new Position(0, 2), out var moveAction2))
        {
            yield return moveAction2;
        }
    }

    private static bool TryGetAttackAction(ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var attackPosition = unitTile.RelativePosition + relativePosition;
        if (!board.TryGetTile(attackPosition, out var targetTile) ||
            !unitTile.CanAttack(targetTile))
        {
            action = FigureAction.None;
            return false;
        }

        if (attackPosition.Y == 7)
        {
            action = new FigureAction(
                FigureActionTypes.Special,
                targetTile.AbsolutePosition,
                () =>
                {
                    unitTile.KillWithoutMove(targetTile, board);
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, CrownsGuardFigureGroup.Blade, unitTile.Figure.IsKing), board);
                    unitTile.Die(board);
                });
            return true;
        }

        action = unitTile.CreateKillWithMove(targetTile, board);
        return true;
    }

    private static bool TryGetMoveAction(ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var movePosition = unitTile.RelativePosition + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !unitTile.CanMoveTo(targetTile))
        {
            action = FigureAction.None;
            return false;
        }

        if (movePosition.Y == 7)
        {
            action = new FigureAction(
                FigureActionTypes.Special,
                targetTile.AbsolutePosition,
                () =>
                {
                    targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, CrownsGuardFigureGroup.Blade, unitTile.Figure.IsKing), board);
                    unitTile.Die(board);
                });
            return true;
        }

        action = unitTile.CreateMoveAction(targetTile, board);
        return true;
    } 
}