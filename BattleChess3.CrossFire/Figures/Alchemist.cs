using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Figures;

public class Alchemist : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 4;

    int IFigureType.FigureId => CrossFireFigureIds.AlchemistId;
    
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
            if (!board.HasTileOnPosition(sourceTile.RelativePosition + movedPosition))
            {
                return;
            }

            var shieldTile = board[sourceTile.RelativePosition + movedPosition];
            if (shieldTile.IsEmpty())
            {
                shieldTile.CreateFigure(new Figure(NeutralPlayerInfo.Instance, CrossFireFigureGroup.Explosives, false), board);
            }
        }
    }
}