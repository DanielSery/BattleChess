using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Figures;

public class Miner : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 3;

    int IFigureType.FigureId => CrossFireFigureIds.MinerId;

    private static readonly Position[] Directions =
    [
        new(0, -1), new(0, 1),
        new(-1, 0), new(1, 0)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var targetTile in Directions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.CanAttack(targetTile))
                yield return unitTile.CreateKillWithMove(targetTile, board);
        }
        
        foreach (var direction in Directions)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanMoveTo(targetTile))
                    yield return new FigureAction(
                        FigureActionTypes.Move,
                        targetTile.AbsolutePosition,
                        () => MoveAction(direction, unitTile, targetTile, board));
                else
                    break;
            }
        }
    }

    private void MoveAction(Position direction, ITile unitTile, ITile targetTile, IBoard board)
    {
        unitTile.MoveToTile(targetTile, board);
        for (var position = unitTile.RelativePosition; position != targetTile.RelativePosition; position += direction)
        {
            if (!board.TryGetTile(position, out var createdTile) ||
                !createdTile.IsEmpty())
            {
                continue;
            }
            
            createdTile.CreateFigure(new Figure(NeutralPlayerInfo.Instance, CrossFireFigureGroup.Trench, false), board);
        }
    }
}