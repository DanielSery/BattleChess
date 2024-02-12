using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.LordOfTheRingsFigures;

internal interface IFigureTypeWithDifferentAttacksAndMoves : IFigureType
{
    protected Position[] MovePositions { get; }

    protected Position[] AttackPositions { get; }

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movementPosition in MovePositions)
        {
            var position = unitTile.Position + movementPosition;
            if (!board.TryGetPovTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }
        }

        foreach (var attackPosition in AttackPositions)
        {
            var position = unitTile.Position + attackPosition;
            if (!board.TryGetPovTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsOwnedByEnemy(unitTile))
            {
                yield return unitTile.CreateKillWithMove(targetTile, board);
            }
        }
    }
}