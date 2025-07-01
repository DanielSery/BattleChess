using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class JapanArcher : ICrossFireFigureType
{
    int IFigureType.FigureId => 26;

    protected Position[] AttackDirections =>
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in AttackDirections)
        {
            for (var i = 1; i <= 3; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetTile(position, out var targetTile))
                    break;

                if (targetTile.IsOwnedByEnemy(unitTile))
                {
                    yield return unitTile.CreateKillWithoutMove(targetTile, board);
                }

                if (targetTile.IsEmpty())
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