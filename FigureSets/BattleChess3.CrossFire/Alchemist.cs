using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures;

public class Alchemist : ICrossFireFigureType
{
    int IFigureType.FigureId => 10;
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var movement in ICrossFireFigureType.NeighbourPositions)
        {
            if (!board.TryGetRelativeTile(unitTile, movement, out var targetTile))
                continue;
            
            if (targetTile.IsEmpty())
            {
                yield return new FigureAction(
                    FigureActionTypes.Move, 
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        CreateExplosive(unitTile, movement, board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }

            if (targetTile.Figure.Type is Explosives)
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    unitTile.AbsolutePosition,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        targetTile.Die(board);
                        CreateExplosive(unitTile, movement, board);
                        unitTile.MoveToTile(targetTile, board);
                    });
            }
        }
    }

    private static void CreateExplosive(ITile sourceTile, Position move, IBoard board)
    {
        var movedPosition = move * 2;
        {
            if (!(sourceTile.Position + movedPosition).IsInBoard())
            {
                return;
            }

            var shieldTile = board[sourceTile.Position + movedPosition];
            if (shieldTile.IsEmpty())
            {
                shieldTile.CreateFigure(new Figure(Player.Neutral, CrossFireFigureGroup.Explosives, false), board);
            }
        }
    }
}