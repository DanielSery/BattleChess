using BattleChess3.CrossFireFigures.Utilities;
using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.CrossFireFigures.Figures;

public class Priest : ICrossFireFigureType
{
    /// <inheritdoc />
    public int FigureValue { get; } = 12;
    
    int IFigureType.FigureId => CrossFireFigureIds.PriestId;
    
    private static readonly Position[] Directions =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    private static readonly Position[] MakeKingPositions =
    [
        new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
    ];

    IEnumerable<FigureAction> IFigureType.GetPossibleActions(ITile unitTile, IBoard board)
    {
        foreach (var direction in Directions)
        {
            foreach (var targetTile in direction.GetRelativeDirectionTiles(1, 7, board, unitTile))
            {
                if (unitTile.CanAttack(targetTile))
                    yield return unitTile.CreateKillWithMove(targetTile, board);
                
                if (unitTile.CanMoveTo(targetTile))
                    yield return unitTile.CreateMoveAction(targetTile, board);
                else
                    break;
            }
        }

        foreach (var targetTile in MakeKingPositions.GetRelativeTiles(board, unitTile))
        {
            if (unitTile.IsAllyTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Special, 
                    targetTile.AbsolutePosition,
                    () => MakeUnitKing(board, targetTile));
            }
            else if (unitTile.IsEnemyTo(targetTile))
            {
                yield return new FigureAction(
                    FigureActionTypes.Special,
                    targetTile.AbsolutePosition,
                    () =>
                    {
                        var figureType = targetTile.Figure.Type;
                        targetTile.Figure.Owner.Figures.Remove(targetTile.Figure);
                        targetTile.CreateFigure(new Figure(unitTile.Figure.Owner, figureType, false), board);
                        MakeUnitKing(board, targetTile);
                    });
            }
        }
    }
    
    

    private void MakeUnitKing(IBoard board, ITile targetTile)
    {
        if (targetTile.Figure.Owner.Equals(PlayerInfo.Neutral))
            return;
        
        var owner = targetTile.Figure.Owner;
        foreach (var checkedTile in board)
        {
            if (!checkedTile.Figure.Owner.Equals(owner) ||
                !checkedTile.Figure.IsKing) 
                continue;
            
            var downgradedFigureBackup = checkedTile.Figure;
            checkedTile.Figure.Owner.Figures.Remove(checkedTile.Figure);
            checkedTile.CreateFigure(new Figure(downgradedFigureBackup.Owner, downgradedFigureBackup.Type, false), board);
        }
        
        var upgradedFigureBackup = targetTile.Figure;
        targetTile.Figure.Owner.Figures.Remove(targetTile.Figure);
        targetTile.CreateFigure(new Figure(upgradedFigureBackup.Owner, upgradedFigureBackup.Type, true), board);
    }
}