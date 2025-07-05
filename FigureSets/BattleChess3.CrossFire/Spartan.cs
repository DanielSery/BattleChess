using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Spartan : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 4;
    
    int IFigureType.FigureId => 12;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        if (unitTile.TryCreateKillWithMove(board, new Position(1, 1), out var attackAction1))
            yield return attackAction1;
        else if (unitTile.TryCreateMoveAction(board, new Position(1, 1), out var moveAction1))
            yield return moveAction1;

        if (unitTile.TryCreateKillWithMove(board, new Position(-1, 1), out var attackAction2))
            yield return attackAction2;
        else if (unitTile.TryCreateMoveAction(board, new Position(-1, 1), out var moveAction2))
            yield return moveAction2;
        
        if (unitTile.TryCreateMoveAction(board, new Position(0, -1), out var moveAction3))
            yield return moveAction3;

        if (unitTile.TryCreateMoveAction(board, new Position(0, 1), out var moveAction4))
        {
            yield return moveAction4;

            if (unitTile.TryCreateMoveAction(board, new Position(0, 2), out var attackAction3))
            {
                yield return attackAction3;
            }
        }
        else
        {
            yield break;
        }

        if (unitTile.Position.Y == 1 &&
            unitTile.TryCreateMoveAction(board, new Position(0, 2), out var moveAction5))
        {
            yield return moveAction5;
        }
    }

}