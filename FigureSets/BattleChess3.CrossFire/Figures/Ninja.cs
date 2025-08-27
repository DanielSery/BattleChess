using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Ninja : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 3;
    
    int IFigureType.FigureId => CrossFireFigureIds.NinjaId;
    
    private static readonly Position[] AttackPositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }

        if (unitTile.TryCreateMoveAction(board, new Position(-1, 1), out var action))
            yield return action;

        if (unitTile.TryCreateMoveAction(board, new Position(1, 1), out action))
            yield return action;

        if (unitTile.CanMoveTo(board, new Position(0, 1)) &&
            unitTile.TryCreateMoveAction(board, new Position(0, 2), out action))
        {
            yield return action;
        }
    }
}