using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Gladiator : ICrossFireFigureType
{
    int IFigureType.FigureId => 34;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (TryGetAttackAction(unitTile, board, new Position(0, 1), out var attackAction2))
        {
            yield return attackAction2;
        }

        if (TryGetMoveAction(unitTile, board, new Position(0, 1), out var move1Action))
        {
            yield return move1Action;
            
            if (TryGetMoveAction(unitTile, board, new Position(0, 2), out var move4Action))
            {
                yield return move4Action;
            }
        }

        if (TryGetMoveAction(unitTile, board, new Position(-1, 0), out var move2Action))
        {
            yield return move2Action;
        }

        if (TryGetMoveAction(unitTile, board, new Position(1, 0), out var move3Action))
        {
            yield return move3Action;
        }
    }

    private static bool TryGetAttackAction(ITile unitTile, IBoard board, Position relativePosition,
        out FigureAction action)
    {
        var attackPosition = unitTile.Position + relativePosition;
        if (!board.TryGetTile(attackPosition, out var targetTile) ||
            !targetTile.IsOwnedByEnemy(unitTile))
        {
            action = FigureAction.None;
            return false;
        }

        action = unitTile.CreateKillWithMove(targetTile, board);
        return true;
    }

    private static bool TryGetMoveAction(ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        var movePosition = unitTile.Position + relativePosition;
        if (!board.TryGetTile(movePosition, out var targetTile) ||
            !targetTile.IsEmpty())
        {
            action = FigureAction.None;
            return false;
        }

        action = unitTile.CreateMoveAction(targetTile, board);
        return true;
    }
}