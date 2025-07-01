using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Bard : ICrossFireFigureType
{
    int IFigureType.FigureId => 11;
    
    private readonly Position[] _movementPositions =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];

    private readonly Position[] _attackPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
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

        foreach (var attackPosition in _attackPositions)
        {
            var position = unitTile.Position + attackPosition;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsOwnedByEnemy(unitTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Attack,
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        var figureType = targetTile.Figure.Type;
                        targetTile.Figure.Owner.Figures.Remove(targetTile.Figure);
                        targetTile.Figure = new Figure(unitTile.Figure.Owner, figureType);
                    });
            }
        }
    }
}