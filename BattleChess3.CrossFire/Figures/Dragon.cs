using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Figures;

public class Dragon : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 12;
    
    int IFigureType.FigureId => CrossFireFigureIds.DragonId;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    private static readonly Position[] FireDirections =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
                yield return unitTile.CreateMoveAction(targetTile, board);
        }
        
        foreach (var neighbourTile in ICrossFireFigureType.NeighbourPositions.GetRelativeTiles(board, unitTile))
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
            targetTile.CreateFigure(new Figure(PlayerInfo.Neutral, CrossFireFigureGroup.Fire, false), board);
        }
        else if (Math.Abs(move.X) <= 2 &&
                 Math.Abs(move.Y) <= 2)
        {
            var smallMove = new Position(Math.Sign(move.X), Math.Sign(move.Y));
            var sourcePosition = unitTile.RelativePosition;
            
            board[sourcePosition + smallMove].CreateFigure(new Figure(PlayerInfo.Neutral, CrossFireFigureGroup.Fire, false), board);
            targetTile.CreateFigure(new Figure(PlayerInfo.Neutral, CrossFireFigureGroup.Fire, false), board);
        }
    }
}