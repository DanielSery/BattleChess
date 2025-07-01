using BattleChess3.CrossFireFigures.Localization;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class CrossFireFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrossFireFigureGroup)}_Name"];

    public static readonly IFigureType Wall = new Wall();
    public static readonly IFigureType Explosives = new Explosives();
    public static readonly IFigureType Knight = new Knight();
    public static readonly IFigureType Trench = new Trench();
    public static readonly IFigureType LegionarySword = new LegionarySword();

    public IFigureType[] FigureTypes { get; } =
    [
        new Ninja(),
        new Crossbow(),
        new OldWizzard(),
        new Builder(),
        new MountedKnight(),
        new Trader(),
        Wall,
        new Barbarian(),
        Explosives,
        new Alchemist(),
        new Bard(),
        new Spartan(),
        new Ranger(),
        new YoungWizzard(),
        new CamelRider(),
        new Scout(),
        new MountedArcher(),
        new CamelArcher(),
        Knight,
        new Chinese(),
        new Warrior(),
        LegionarySword,
        new LegionaryPike(),
        new King(),
        new Queen(),
        new Viking(),
        new JapanArcher(),
        new Archer(),
        new Trench(),
        new Miner(),
        new Peasant(),
        new Spearman(),
        new Warhammer(),
        new Gladiator()
    ];
}