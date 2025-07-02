using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Cannon : ICrossFireFigureType
{
    int IFigureType.FigureId => 36;
    
    private readonly Position[] _attackPositions =
    [
        new(0, 2), 
        new(-1, 3), new(0, 3), new(1, 3),
        new(-1, 4), new(0, 4), new(1, 4),
    ];
    
    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (board.TryGetRelativeTile(unitTile, new Position(0, 1), out var tileBefore1) && 
            !tileBefore1.IsEmpty())
        {
            yield break;
        }
        
        if (TryGetAttackAction(unitTile, board, new Position(0, 2), out var attack1Action))
        {
            yield return attack1Action;
        }
        
        if (TryGetAttackAction(unitTile, board, new Position(0, 3), out var attack2Action))
        {
            yield return attack2Action;
        }
        
        if (TryGetAttackAction(unitTile, board, new Position(0, 4), out var attack3Action))
        {
            yield return attack3Action;
        }
    }

    private bool TryGetAttackAction(ITile unitTile, IBoard board, Position relativePosition, out FigureAction action)
    {
        if (!board.TryGetRelativeTile(unitTile, relativePosition, out var targetTile) ||
            targetTile.IsEmpty())
        {
            action = FigureAction.None;
            return false;
        }

        action = new FigureAction(
            FigureActionTypes.Attack,
            unitTile.AbsolutePosition,
            targetTile.AbsolutePosition,
            () =>
            {
                foreach (var attackPosition in _attackPositions)
                {
                    if (board.TryGetRelativeTile(unitTile, attackPosition, out var tile) &&
                        !tile.IsEmpty())
                    {
                        unitTile.KillWithoutMove(tile, board);
                    }
                }
            });
        return true;
    }
}