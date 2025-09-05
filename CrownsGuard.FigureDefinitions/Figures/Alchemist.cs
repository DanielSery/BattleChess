
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Alchemist : ICrossFireFigureType
{
    public int FigureValue => 4;

    public int FigureId => CrossFireFigureIds.AlchemistId;
    
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
                shieldTile.CreateFigure(new Figure(NeutralFigureOwner.Instance, CrossFireFigureGroup.Explosives, false), board);
            }
        }
    }
}