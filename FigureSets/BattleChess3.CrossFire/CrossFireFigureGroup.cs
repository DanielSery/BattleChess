using BattleChess3.Core.Model.Figures;
using BattleChess3.CrossFireFigures.Localization;

namespace BattleChess3.CrossFireFigures;

public class CrossFireFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrossFireFigureGroup)}_Name"];

    public static readonly IFigureType Ninja = new Ninja();
    public static readonly IFigureType Bomber = new Bomber();
    public static readonly IFigureType Builder = new Builder();
    public static readonly IFigureType Knight = new Knight();
    public static readonly IFigureType Spy = new Spy();
    public static readonly IFigureType Archer = new Archer();
    public static readonly IFigureType Wall = new Wall();

    public IFigureType[] FigureTypes =>
        new[]
        {
            Ninja,
            Bomber,
            Builder,
            Knight,
            Spy,
            Archer,
            Wall
        };
}