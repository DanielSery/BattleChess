using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Nordguard: ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 8;
    
    int IFigureType.FigureId => CrossFireFigureIds.NordguardId;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, -1), new(1, -1), new(-1, 1), new(1, 1)
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