using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Whiplash : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 6;
    
    int IFigureType.FigureId => CrossFireFigureIds.WhiplashId;
    
    private static readonly Position[] AttackMovePositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
    
    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in AttackMovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);

            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }
}