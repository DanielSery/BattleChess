using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.CrossFireFigures.Utilities;

namespace BattleChess3.CrossFireFigures.Figures;

public class Trader : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 7;
    
    int IFigureType.FigureId => CrossFireFigureIds.TraderId;
    
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
                    targetTile.AbsolutePosition,
                    () => unitTile.SwapWithTile(targetTile, board));
            }
        }
    }
}