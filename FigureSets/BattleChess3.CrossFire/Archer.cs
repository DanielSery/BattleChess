using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Archer : ICrossFireFigureType
{
    int IFigureType.FigureId => 27;

    protected Position[] AttackDirections =>
    [
        new(-1, 0), new(1, 0),
        new(0, -1), new(0, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in AttackDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;

                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithoutMove(targetTile, board);
                
                if (unitTile.CanMoveTo(targetTile))
                {
                    if (i <= 2)
                    {
                        yield return unitTile.CreateMoveAction(targetTile, board);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}