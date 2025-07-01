using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures;

public class Miner : ICrossFireFigureType
{
    int IFigureType.FigureId => 29;
    
    private readonly Position[] _movePosition =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in _movePosition)
        {
            if (!board.TryGetRelativeTile(unitTile, movement, out var targetTile))
                continue;
            
            if (unitTile.CanMoveTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Move, 
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        MoveShield(unitTile, movement, board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }

            if (targetTile.Figure.Type is Trench)
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        targetTile.Die(board);
                        MoveShield(unitTile, movement, board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }
        }
    }

    private static void MoveShield(ITile sourceTile, Position move, IBoard board)
    {
        var movedPositions = GetMovedPositions(move);
        foreach (var targetTile in movedPositions.GetRelativeTiles(board, sourceTile))
        {
            if (targetTile.IsEmpty())
                targetTile.CreateFigure(new Figure(Player.Neutral, CrossFireFigureGroup.Trench), board);
        }
    }

    private static Position[] GetMovedPositions(Position move)
    {
        return move switch
        {
            (0, 1) => [new Position(-1, 2), new Position(0, 2), new Position(1, 2)],
            (1, 0) => [new Position(2, -1), new Position(2, 0), new Position(2, 1)],
            (0, -1) => [new Position(-1, -2), new Position(0, -2), new Position(1, -2)],
            (-1, 0) => [new Position(-2, -1), new Position(-2, 0), new Position(-2, 1)],
            _ => throw new ArgumentException($"Unexpected move of Builder {move}")
        };
    }
}