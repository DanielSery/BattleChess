using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.DefaultFigures;

public class Palm : IDefaultFigureType
{
    public static readonly Palm Instance = new();

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return FigureAction.None;
    }
}
