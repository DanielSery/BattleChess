using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Bard : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 12;
    
    int IFigureType.FigureId => CrossFireFigureIds.BardId;
    
    private static readonly Position[] MovementPositions =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    private static readonly Position[] AttackPositions =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovementPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }

        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Attack,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        var figureType = targetTile.Figure.Type;
                        targetTile.Figure.Owner.Figures.Remove(targetTile.Figure);
                        targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, figureType, false), board);
                    });
            }
        }
    }
}