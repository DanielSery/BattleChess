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
        new Gladiator(),
        new Ninja(),
        LegionarySword,
        new LegionaryPike(),
        new Spartan(),
        
        new Warhammer(),
        new King(),
        new Viking(),
        Knight,
        new MountedKnight(),
        new Chinese(),
        new CamelRider(),
        new Scout(),
        new MountedArcher(),
        new CamelArcher(),
        new Elephant(),
        
        new Crossbow(),
        new Ranger(),
        new JapanArcher(),
        new Archer(),
        new Musketeer(),
        new Cannon(),
        new Catapult(),
        
        new Builder(),
        new Trader(),
        new Bard(),
        new Alchemist(),
        new Barbarian(),
        new Miner(),
        new Priest(),
        
        new OldWizzard(),
        new YoungWizzard(),
        new Warrior(),
        new Queen(),
        new Dragon()
    ];
}