using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.StarWarsFigures;

public class YodaDooku : IStarWarsFigureType
{
    private readonly Position[] _movementPositions = 
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };
    
    private readonly Position[] _attackDirections = 
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movementPosition in _movementPositions)
        {
            var position = unitTile.Position + movementPosition;
            if (!board.TryGetPovTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }

            if (targetTile.Figure.FigureType is Bomb &&
                targetTile.IsOwnedByYou(unitTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        targetTile.Die(board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }
        }
        
        foreach (var direction in _attackDirections)
        {
            var movedTile = NoneTile.Instance;
            for (var i = 1; i < 8; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetPovTile(position, out var targetTile))
                    break;

                if (!targetTile.IsEmpty())
                {
                    movedTile = targetTile;
                    break;
                }
            }

            if (movedTile == NoneTile.Instance)
            {
                continue;
            }
            
            for (var i = 1; i < 8; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetPovTile(position, out var targetTile))
                    break;

                if (targetTile.IsEmpty())
                {
                    yield return movedTile.CreateMoveAction(targetTile, board);
                }
                else if (targetTile != NoneTile.Instance)
                {
                    break;
                }
            }
        }
    }
}