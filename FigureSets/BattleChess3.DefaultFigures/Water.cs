using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DefaultFigures;

public class Water : IDefaultFigureType
{
    public static Water Instance { get; } = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return FigureAction.None;
    }
}