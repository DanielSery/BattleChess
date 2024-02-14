using BattleChess3.DefaultFigures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.History;

internal class HistoryService : IHistoryService
{
    public IList<RecordedAction> RecordedActions { get; } = new List<RecordedAction>();

    public HistoryService(IFigureCreator)
    {
        
    }
    
    public void RecordAction(
        Position sourcePosition,
        Position targetPosition,
        Figure figure,
        RecordedActionType actionType)
    {
        RecordedActions.Add(new RecordedAction(sourcePosition, targetPosition, figure, actionType));
    }

    public void RevertLastAction(IBoard board)
    {
        var action = RecordedActions[^1];
        RecordedActions.RemoveAt(RecordedActions.Count - 1);
        
        switch (action.ActionType)
        {
            case RecordedActionType.Moved:
            {
                var sourceTile = board[action.SourcePosition];
                var targetTile = board[action.TargetPosition];
                (sourceTile.Figure, targetTile.Figure) = (targetTile.Figure, sourceTile.Figure);
                break;
            }
            case RecordedActionType.Created:
            {
                var targetTile = board[action.TargetPosition];
                targetTile.Figure.Owner.Figures.Remove(targetTile.Figure);
                targetTile.Figure = new Figure(Player.Neutral, DefaultFigureGroup.Empty);
                break;
            }
            case RecordedActionType.Destroyed:
            {
                
            }
        }
    }

    public IEnumerable<RecordedAction> GetHistory()
    {
        return RecordedActions;
    }
}