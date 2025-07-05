using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Gladiator : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 3;
    
    int IFigureType.FigureId => 34;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (unitTile.TryCreateKillWithMove(board, new Position(-1, 1), out var action))
        {
            yield return action;
        }
        
        if (unitTile.TryCreateKillWithMove(board, new Position(1, 1), out action))
        {
            yield return action;
        }

        if (unitTile.TryCreateMoveAction(board, new Position(0, 1), out action))
        {
            yield return action;
            
            if (unitTile.TryCreateMoveAction(board, new Position(0, 2), out action))
            {
                yield return action;
            }
        }

        if (unitTile.TryCreateMoveAction(board, new Position(-1, 0), out action))
        {
            yield return action;
        }

        if (unitTile.TryCreateMoveAction(board, new Position(1, 0), out action))
        {
            yield return action;
        }
    }
}