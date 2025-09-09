using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Dragon : ICrownsGuardFigureType
{
    public int FigureValue => 12;

    public int FigureId => (int)CrownsGuardFigureIds.DragonId;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    private static readonly Position[] FireDirections =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var neighbourTile in ICrownsGuardFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsEnemyTo(neighbourTile))
                yield break;
        }
        
        foreach (var direction in FireDirections)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 2, board, unitTile))
            {
                if (targetTile.IsEmpty())
                {
                    yield return new FigureAction(
                        FigureActionTypes.Special, 
                        targetTile.AbsolutePosition,
                        () => FireAction(unitTile, targetTile, board));
                }
                else 
                {
                    break;
                }
            }
        }
    }

    private void FireAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var move = targetTile.RelativePosition - unitTile.RelativePosition;

        if (Math.Abs(move.X) <= 1 &&
            Math.Abs(move.Y) <= 1)
        {
            targetTile.CreateFigure(new Figure(NeutralFigureOwner.Instance, CrownsGuardFigureGroup.Fire, false), board);
        }
        else if (Math.Abs(move.X) <= 2 &&
                 Math.Abs(move.Y) <= 2)
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.RelativePosition;
            
            board[sourcePosition + smallMove].CreateFigure(new Figure(NeutralFigureOwner.Instance, CrownsGuardFigureGroup.Fire, false), board);
            targetTile.CreateFigure(new Figure(NeutralFigureOwner.Instance, CrownsGuardFigureGroup.Fire, false), board);
        }
    }
}