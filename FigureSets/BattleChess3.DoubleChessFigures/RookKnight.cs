using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DoubleChessFigures;

public class RookKnight : IDoubleChessFigureType
{
    public static readonly RookKnight Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return (this as IDoubleChessFigureType).CreatedCombinedActions(
            unitTile,
            targetTile,
            board,
            SingleRook.Instance,
            RookKnight.Instance);
    }
}