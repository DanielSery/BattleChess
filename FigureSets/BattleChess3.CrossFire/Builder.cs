using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Builder : ICrossFireFigureType
{
    int IFigureType.FigureId => 4;
    
    private readonly Position[] _movePosition =
    [
        new(-1, -1), new(1, -1), new(1, 1), new(-1, 1)
    ];
    
    private readonly Position[] _shieldPositions =
    [
        new(-2, 1), new(-2, -1),
        new(-1, -2), new(-1, 0), new (-1, 2),
        new(0, -1), new(0, 1),
        new(1, -2), new(1, 0),  new(1, 2),
        new(2, -1), new(2, 1),
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in _movePosition)
        {
            var position = unitTile.Position + movement;
            if (!board.TryGetTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }

        foreach (var shieldPosition in _shieldPositions)
        {
            var position = unitTile.Position + shieldPosition;
            if (!board.TryGetTile(position, out var targetTile))
                continue;

            if (targetTile.IsEmpty())
            {
                yield return new FigureAction(
                    FigureActionTypes.Special, 
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, CrossFireFigureGroup.Wall), board);
                    });
            }
        }
    }
}