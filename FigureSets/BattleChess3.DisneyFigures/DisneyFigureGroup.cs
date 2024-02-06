using BattleChess3.Core.Model.Figures;
using BattleChess3.DisneyFigures.Localization;

namespace BattleChess3.DisneyFigures;

public class DisneyFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(DisneyFigureGroup)}_Name"];

    public IFigureType[] FigureTypes => new IFigureType[]
    {
        DisneyKing.Instance,
        DisneyQueen.Instance,
        DisneyRook.Instance,
        DisneyBishop.Instance,
        DisneyKnight.Instance,
        DisneyPawn.Instance
    };
}