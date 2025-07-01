using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Catapult : ICrossFireFigureType
{
    int IFigureType.FigureId => 37;
    
    private readonly Position[] _attackPositions =
    [
        new (-1, 2), new (1, 2),
        new (-2, 3), new (0, 3), new (2, 3),
        new (-1, 4), new (1, 4),
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in _attackPositions.GetRelativeTiles(board, unitTile))
        {
            if (!targetTile.IsEmpty())
                yield return unitTile.CreateKillWithoutMove(targetTile, board);
        }
    }
}