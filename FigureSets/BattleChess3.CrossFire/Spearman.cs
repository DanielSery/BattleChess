using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Spearman : ICrossFireFigureType
{
    int IFigureType.FigureId => 32;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (unitTile.CanMoveTo(board, new Position(0, 1)) &&
            unitTile.TryCreateKillWithMove(board, new Position(0, 2), out var attackAction1))
        {
            yield return attackAction1;
        }
        
        if (unitTile.TryCreateKillWithMove(board, new Position(0, 1), out var attackAction2))
            yield return attackAction2;

        if (unitTile.TryCreateMoveAction(board, new Position(0, 1), out var move1Action))
            yield return move1Action;

        if (unitTile.TryCreateMoveAction(board, new Position(-1, 0), out var move2Action))
            yield return move2Action;

        if (unitTile.TryCreateMoveAction(board, new Position(1, 0), out var move3Action))
            yield return move3Action;
    }
}