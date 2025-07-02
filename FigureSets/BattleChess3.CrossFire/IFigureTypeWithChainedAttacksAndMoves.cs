using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

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
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;
                
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithMove(targetTile, board);

                if (!unitTile.CanMoveTo(targetTile))
                    break;
            }
        }
        
        foreach (var direction in MoveDirections)
        {
            for (var i = 1; i < 8; i++)
            {
                if (!board.TryGetRelativeTile(unitTile, direction * i, out var targetTile))
                    break;
                
                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
                else
                    break;
            }
        }
    }
}