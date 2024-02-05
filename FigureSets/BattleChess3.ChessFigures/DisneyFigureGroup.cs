using BattleChess3.ChessFigures.Localization;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.ChessFigures;

public class DisneyFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(DisneyFigureGroup)}_Name"];

    public IFigureType[] FigureTypes => new IFigureType[]
    {
        King.Instance,
        Queen.Instance,
        Rook.Instance,
        Bishop.Instance,
        Knight.Instance,
        Pawn.Instance,
    };
}
