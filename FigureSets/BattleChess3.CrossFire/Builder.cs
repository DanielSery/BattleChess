using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures;

public class Builder : ICrossFireFigureType
{
    int IFigureType.FigureId => 4;
    
    private readonly Position[] _movePosition =
    [
        new(-1, -1), new(1, -1), new(1, 1), new(-1, 1)
    ];
    
    private readonly Position[] _shieldPositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in _movePosition.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }

        foreach (var targetTile in _shieldPositions.GetRelativeTiles(board, unitTile))
        {
            if (targetTile.IsEmpty())
                yield return unitTile.CreateNewFigureAction(targetTile, Player.Neutral, CrossFireFigureGroup.Wall, board);

            if (targetTile.Figure.Type is Wall)
                yield return unitTile.CreateKillWithoutMove(targetTile, board);
        }
    }
}