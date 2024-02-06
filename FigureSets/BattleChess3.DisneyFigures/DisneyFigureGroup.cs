using BattleChess3.Core.Model.Figures;
using BattleChess3.DisneyFigures.Localization;

namespace BattleChess3.DisneyFigures;

public class DisneyFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(DisneyFigureGroup)}_Name"];

    public static readonly IFigureType DisneyKing = new DisneyKing();
    public static readonly IFigureType DisneyQueen = new DisneyQueen();
    public static readonly IFigureType DisneyRook = new DisneyRook();
    public static readonly IFigureType DisneyBishop = new DisneyBishop();
    public static readonly IFigureType DisneyKnight = new DisneyKnight();
    public static readonly IFigureType DisneyPawn = new DisneyPawn();

    public IFigureType[] FigureTypes =>
        new[]
        {
            DisneyKing,
            DisneyQueen,
            DisneyRook,
            DisneyBishop,
            DisneyKnight,
            DisneyPawn
        };
}