using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.FigureDefinitions.Figures;

public class Priest : ICrownsGuardFigureType
{
    public int FigureValue => 12;

    private static readonly Position[] Directions =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];

    private static readonly Position[] MakeKingPositions =
    [
        new(1, 0), new(0, 1), new(-1, 0), new(0, -1)
    ];

    public static FigureAction[] GetPossibleActions(Position sourcePosition, Figure sourceFigure, Figure[] board)
    {
        Span<FigureAction> actions = stackalloc FigureAction[36];
        int actionsCount = 0;
        
        return actions.ToArrayPool(actionsCount);
    }

    public static void ExecuteAction(Figure[] board, ref FigureAction action, Action<BoardEvent, Figure[]> onEvent)
    {
        
    }

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
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
                        targetTile.CreateFigure(new FigureInfo(unitTile.Figure.Owner, figureType, false), board);
                        MakeUnitKing(board, targetTile);
                    });
            }
        }
    }
    
    

    private void MakeUnitKing(IBoard board, ITile targetTile)
    {
        if (targetTile.Figure.Owner.Equals(NeutralFigureOwner.Instance))
            return;
        
        var owner = targetTile.Figure.Owner;
        foreach (var checkedTile in board)
        {
            if (!checkedTile.Figure.Owner.Equals(owner) ||
                !checkedTile.Figure.IsKing) 
                continue;
            
            var downgradedFigureBackup = checkedTile.Figure;
            checkedTile.Figure.Owner.Figures.Remove(checkedTile.Figure);
            checkedTile.CreateFigure(new FigureInfo(downgradedFigureBackup.Owner, downgradedFigureBackup.Type, false), board);
        }
        
        var upgradedFigureBackup = targetTile.Figure;
        targetTile.Figure.Owner.Figures.Remove(targetTile.Figure);
        targetTile.CreateFigure(new FigureInfo(upgradedFigureBackup.Owner, upgradedFigureBackup.Type, true), board);
    }
}