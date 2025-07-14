using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Peasant : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 2;
    
    int IFigureType.FigureId => 31;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (unitTile.TryCreateMoveAction(board, new Position(0, 1), out var attackAction))
            yield return attackAction;

        if (unitTile.TryCreateKillWithMove(board, new Position(0, 1), out var move1Action))
            yield return move1Action;

        if (unitTile.TryCreateKillWithMove(board, new Position(-1, 0), out var move2Action))
            yield return move2Action;

        if (unitTile.TryCreateKillWithMove(board, new Position(1, 0), out var move3Action))
            yield return move3Action;
    }
}