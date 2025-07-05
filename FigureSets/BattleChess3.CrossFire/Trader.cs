using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Trader : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 7;
    
    int IFigureType.FigureId => 6;
    
    private static readonly Position[] AttackMovePositions =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in AttackMovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);

            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }

        foreach (var targetTile in board)
        {
            if (targetTile.IsAllyTo(unitTile) &&
                targetTile.Figure != unitTile.Figure)
            {
                yield return new FigureAction(
                    FigureActionTypes.Special,
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () => unitTile.MoveToTile(targetTile, board));
            }
        }
    }
}