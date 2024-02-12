using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;

namespace BattleChess3.LordOfTheRingsFigures;

internal interface IFigureTypeWithChainedAttacksAndMoves : IFigureType
{
    protected Position[] MoveDirections { get; }
    protected Position[] AttackDirections { get; }

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in AttackDirections)
        {
            for (var i = 1; i < 8; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetPovTile(position, out var targetTile))
                    break;
                
                if (targetTile.IsOwnedByEnemy(unitTile))
                {
                    yield return unitTile.CreateKillWithMove(targetTile, board);
                }

                if (!targetTile.IsEmpty())
                {
                    break;
                }
            }
        }
        
        foreach (var direction in MoveDirections)
        {
            for (var i = 1; i < 8; i++)
            {
                var position = unitTile.Position + direction * i;
                if (!board.TryGetPovTile(position, out var targetTile))
                    break;
                
                if (targetTile.IsEmpty())
                {
                    yield return unitTile.CreateMoveAction(targetTile, board);
                }
                else
                {
                    break;
                }
            }
        }
    }
}