using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Barbarian : ICrossFireFigureType
{
    int IFigureType.FigureId => 8;
    
    private readonly Position[] _movementPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 0), new(-1, 2),
        new(0, -1), new(0, 1),
        new(1, -2), new(1, 0), new(1, 2),
        new(2, -1), new(2, 1)
    ];
    
    private readonly Position[] _attackDirections =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movementPosition in _movementPositions)
        {
            var position = unitTile.Position + movementPosition;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }
        
        foreach (var direction in _attackDirections)
        {
            var movedTile = NoneTile.Instance;
            for (var i = 1; i <= 1; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetTile(position, out var targetTile))
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
                if (!board.TryGetTile(position, out var targetTile))
                    break;

                if (targetTile.Position == movedTile.Position)
                {
                }
                else if (targetTile.IsEmpty())
                {
                    yield return new FigureAction(
                        FigureActionTypes.Special,
                        unitTile.AbsolutePosition,
                        targetTile.AbsolutePosition,
                        () => movedTile.MoveToTile(targetTile, board));
                }
                else if (targetTile != NoneTile.Instance)
                {
                    break;
                }
            }
        }
    }
}