using BattleChess3.Game.GameBoard;

namespace BattleChess3.Game.Figures;

public class FigureAction
{
    public static readonly FigureAction None = new(FigureActionTypes.None, Position.None, () => { });

    public FigureAction(
        FigureActionTypes actionType,
        Position targetPosition,
        Action action)
    {
        ActionType = actionType;
        TargetPosition = targetPosition;
        Action = action;
    }

    public Position TargetPosition { get; }
    public FigureActionTypes ActionType { get; }
    public Action Action { get; }
}