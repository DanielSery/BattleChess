using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Builder : ICrossFireFigureType
{
    public int FigureValue => 4;

    public int FigureId => CrossFireFigureIds.BuilderId;
    
    private static readonly Position[] MovePosition =
    [
        new(-1, -1), new(1, -1), new(1, 1), new(-1, 1)
    ];
    
    private static readonly Position[] ShieldPositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePosition.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }

        foreach (var targetTile in ShieldPositions.GetRelativeTiles(board, unitTile))
        {
            if (targetTile.IsEmpty())
                yield return unitTile.CreateNewFigureAction(targetTile, NeutralFigureOwner.Instance, CrossFireFigureGroup.Wall, board);

            if (targetTile.Figure.Type is Wall)
                yield return unitTile.CreateKillWithoutMove(targetTile, board);
        }
    }
}