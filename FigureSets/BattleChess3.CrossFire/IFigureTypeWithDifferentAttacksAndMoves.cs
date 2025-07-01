using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

internal interface IFigureTypeWithDifferentAttacksAndMoves : IFigureType
{
    protected Position[] MovePositions { get; }

    protected Position[] AttackPositions { get; }

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movementPosition in MovePositions)
        {
            if (!board.TryGetRelativeTile(unitTile, movementPosition, out var targetTile))
                continue;
            
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }

        foreach (var attackPosition in AttackPositions)
        {
            if (!board.TryGetRelativeTile(unitTile, attackPosition, out var targetTile))
                continue;
            
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }
}