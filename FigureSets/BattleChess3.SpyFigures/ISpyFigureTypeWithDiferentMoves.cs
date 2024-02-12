using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.SpyFigures;

internal interface ISpyFigureTypeWithDifferentAttackMoves : IFigureType
{
    protected Position[] AttackMovePositions { get; }

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in AttackMovePositions)
        {
            var position = unitTile.Position + movement;
            if (!board.TryGetPovTile(position, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return unitTile.CreateMoveAction(targetTile, board);
            }
            else
            {
                yield return unitTile.CreateKillWithMove(targetTile, board);
            }
        }
    }
}