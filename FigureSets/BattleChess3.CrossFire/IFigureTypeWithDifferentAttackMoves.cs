using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

internal interface IFigureTypeWithDifferentAttackMoves : IFigureType
{
    protected Position[] AttackMovePositions { get; }

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in AttackMovePositions)
        {
            if (!board.TryGetRelativeTile(unitTile, movement, out var targetTile))
                continue;
            
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);

            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
    }
}