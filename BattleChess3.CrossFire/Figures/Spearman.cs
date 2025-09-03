using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Spearman : ICrossFireFigureType
{
    public int FigureValue => 3;

    public int FigureId => CrossFireFigureIds.SpearmanId;

    private static readonly Position[] AttackPositions =
    [
        new(-1, 1), new(1, 1), new(-1, -1), new(1, -1)
    ];

    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0), new(0, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
        
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
    }
}