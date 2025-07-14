using BattleChess3.CrossFireFigures.Localization;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class CrossFireFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrossFireFigureGroup)}_Name"];

    public static readonly IFigureType Wall = new Wall();
    public static readonly IFigureType Explosives = new Explosives();
    public static readonly IFigureType Blade = new Blade();
    public static readonly IFigureType Trench = new Trench();
    public static readonly IFigureType LegionarySword = new LegionarySword();
    public static readonly IFigureType Empty = new Empty();
    public static readonly IFigureType Fire = new Fire();

    public IFigureType[] FigureTypes { get; } =
    [
        Empty,
        Wall,
        Explosives,
        Trench,
        Fire,
        
        new Peasant(),
        new Spearman(),
        new Pikeman(),
        LegionarySword,
        new LegionaryPike(),
        
        new MountedKnight(),
        new CamelRider(),
        new MountedArcher(),
        new CamelArcher(),
        new Scout(),
        new Dogs(),
        new Queen(),
        
        new Knight(),
        new Samurai(),
        new Chinese(),
        new Nordguard(),
        Blade,
        new Elephant(),
        
        new Archer(),
        new JapanArcher(),
        new Ranger(),
        new Crossbow(),
        new Musketeer(),
        new Cannon(),
        new Catapult(),
        
        new Spartan(),
        new Warhammer(),
        new BattleAxe(),
        new Mage(),
        new Wizzard(),
        
        new King(),
        new Trader(),
        new Bard(),
        new Barbarian(),
        new Whiplash(),
        new Priest(),
        
        new Builder(),
        new Alchemist(),
        new Miner(),
        new Dragon()
    ];
}