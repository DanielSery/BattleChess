using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.CrossFireFigures.Figures;

public class Warhammer : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 6;
    
    int IFigureType.FigureId => CrossFireFigureIds.WarhammerId;
    
    private static readonly Position[] MovePositions =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
    
    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in MovePositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanMoveTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Move,
                    targetTile.AbsolutePosition,
                    () => MoveAction(unitTile, targetTile, board));
            }
        }
    }

    private void MoveAction(ITile unitTile, ITile targetTile, IBoard board)
    {
        unitTile.MoveToTile(targetTile, board);
        var movement = targetTile.RelativePosition - unitTile.RelativePosition;
        switch (movement)
        {
            case { Y: 0, X: 1 }:
                targetTile.TryDestroyTile(board, new Position(1, -1));
                targetTile.TryDestroyTile(board, new Position(1, 0));
                targetTile.TryDestroyTile(board, new Position(1, 1));
                break;
            case { Y: 0, X: -1 }:
                targetTile.TryDestroyTile(board, new Position(-1, -1));
                targetTile.TryDestroyTile(board, new Position(-1, 0));
                targetTile.TryDestroyTile(board, new Position(-1, 1));
                break;
            case { Y: 1, X: 0 }:
                targetTile.TryDestroyTile(board, new Position(-1, 1));
                targetTile.TryDestroyTile(board, new Position(0, 1));
                targetTile.TryDestroyTile(board, new Position(1, 1));
                break;
            case { Y: -1, X: 0 }:
                targetTile.TryDestroyTile(board, new Position(-1, -1));
                targetTile.TryDestroyTile(board, new Position(0, -1));
                targetTile.TryDestroyTile(board, new Position(1, -1));
                break;
        }
    }
}