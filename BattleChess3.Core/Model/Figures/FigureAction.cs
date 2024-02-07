namespace BattleChess3.Core.Model.Figures;

public class FigureAction
{
    public static readonly FigureAction None = new(FigureActionTypes.None, () => { });

    public FigureAction(FigureActionTypes actionType, Action action)
    {
        ActionType = actionType;
        Action = action;
    }

    public FigureActionTypes ActionType { get; }
    public Action Action { get; }
}