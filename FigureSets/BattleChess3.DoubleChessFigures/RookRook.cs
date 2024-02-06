using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DoubleChessFigures;

public class RookRook : IDoubleChessFigureType
{
    public static readonly RookRook Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return (this as IDoubleChessFigureType).CreatedCombinedActions(
            unitTile,
            targetTile,
            board,
            SingleRook.Instance,
            RookRook.Instance);
    }
}