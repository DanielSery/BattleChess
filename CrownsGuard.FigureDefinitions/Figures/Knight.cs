using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Knight : ICrownsGuardFigureType
{
    public int FigureValue => 10;

    public int FigureId => (int)CrownsGuardFigureIds.KnightId;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    private static readonly Position[] AttackDirections =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var direction in AttackDirections)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 3, board, unitTile))
            {
                if (unitTile.CanAttack(targetTile))
                {
                    yield return new FigureAction(
                        FigureActionTypes.Attack,
                        targetTile.AbsolutePosition,
                        () => AttackAction(unitTile, targetTile, board));
                }
                else if (!unitTile.CanMoveTo(targetTile))
                {
                    break;
                }
            }
        }
    }

    private void AttackAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        var move = targetTile.RelativePosition - unitTile.RelativePosition;

        if (Math.Abs(move.X) <= 1 &&
            Math.Abs(move.Y) <= 1)
        {
            unitTile.KillWithMove(targetTile, board);
        }
        else if (Math.Abs(move.X) <= 2 &&
                 Math.Abs(move.Y) <= 2)
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.RelativePosition;
            
            unitTile.KillWithMove(board[sourcePosition + smallMove], board);
            unitTile = board[sourcePosition + smallMove];
            if (!unitTile.Figure.Type.Equals(this))
                return;
           
            unitTile.KillWithMove(targetTile, board); 
        }
    }
}