using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Knight : ICrossFireFigureType
{
    int IFigureType.FigureId => 19;
    
    private static readonly Position[] MoveDirections =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    private static readonly Position[] AttackPositions =
    [
        new(-2, -2), new(-1, -1),
        new(-2, 2), new(-1, 1),
        new(2, -2), new(1, -1),
        new(2, 2), new(1, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MoveDirections.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }

        foreach (var targetTile in AttackPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }
}