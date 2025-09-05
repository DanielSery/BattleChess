using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions;

public sealed class CrossFireFigureGroup : IFigureGroup
{
    private readonly Dictionary<int, IFigureType> _figuresDictionary;
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrossFireFigureGroup)}_Name"];

    internal static readonly IFigureType Wall = new Wall();
    internal static readonly IFigureType Explosives = new Explosives();
    internal static readonly IFigureType Blade = new Blade();
    internal static readonly IFigureType Trench = new Trench();
    internal static readonly IFigureType LegionarySword = new LegionarySword();
    internal static readonly IFigureType Empty = new Empty();
    internal static readonly IFigureType Fire = new Fire();

    public CrossFireFigureGroup()
    {
        _figuresDictionary = FigureTypes.ToDictionary(figure => figure.FigureId, figure => figure);
    }

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

    /// <inheritdoc />
    public IFigureType GetFigureTypeById(int uniqueUnitId)
    {
        return _figuresDictionary[uniqueUnitId];
    }
}