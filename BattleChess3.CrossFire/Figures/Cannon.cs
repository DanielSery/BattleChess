using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Cannon : ICrossFireFigureType
{
    public int FigureValue => 12;

    public int FigureId => CrossFireFigureIds.CannonId;
    
    private static readonly Position[] AttackPositions =
    [
        new(0, 2), new(0, 3), new(0, 4),
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
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
            targetTile.AbsolutePosition,
            () =>
            {
                foreach (var attackPosition in AttackPositions)
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