using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions;

public sealed class CrownsGuardFigureTypeInfoGroup : IFigureTypeInfoGroup
{
    private readonly Dictionary<Figure, IFigureTypeInfo> _figuresDictionary;
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrownsGuardFigureTypeInfoGroup)}_Name"];

    public CrownsGuardFigureTypeInfoGroup()
    {
        _figuresDictionary = new Dictionary<Figure, IFigureTypeInfo>()
        {
            { Figure.Empty, new Empty() },
            { Figure.Wall, new Wall() },
            { Figure.Explosives, new Explosives() },
            { Figure.Trench, new Trench() },
            { Figure.Fire, new Fire() },

            { Figure.Peasant, new Peasant() },
            { Figure.Spearman, new Spearman() },
            { Figure.Pikeman, new Pikeman() },
            { Figure.LegionarySword, new LegionarySword() },
            { Figure.LegionaryPike, new LegionaryPike() },

            { Figure.MountedKnight, new MountedKnight() },
            { Figure.CamelRider, new CamelRider() },
            { Figure.MountedArcher, new MountedArcher() },
            { Figure.CamelArcher, new CamelArcher() },
            { Figure.Scout, new Scout() },
            { Figure.Dogs, new Dogs() },
            { Figure.Queen, new Queen() },

            { Figure.Knight, new Knight() },
            { Figure.Samurai, new Samurai() },
            { Figure.Chinese, new Chinese() },
            { Figure.Nordguard, new Nordguard() },
            { Figure.Blade, new Blade() },
            { Figure.Elephant, new Elephant() },

            { Figure.Archer, new Archer() },
            { Figure.JapanArcher, new JapanArcher() },
            { Figure.Ranger, new Ranger() },
            { Figure.Crossbow, new Crossbow() },
            { Figure.Musketeer, new Musketeer() },
            { Figure.Cannon, new Cannon() },
            { Figure.Catapult, new Catapult() },

            { Figure.Spartan, new Spartan() },
            { Figure.Warhammer, new Warhammer() },
            { Figure.BattleAxe, new BattleAxe() },
            { Figure.Mage, new Mage() },
            { Figure.Wizzard, new Wizzard() },

            { Figure.King, new King() },
            { Figure.Trader, new Trader() },
            { Figure.Bard, new Bard() },
            { Figure.Barbarian, new Barbarian() },
            { Figure.Whiplash, new Whiplash() },
            { Figure.Priest, new Priest() },

            { Figure.Builder, new Builder() },
            { Figure.Alchemist, new Alchemist() },
            { Figure.Miner, new Miner() },
            { Figure.Dragon, new Dragon() },
        };
        FigureTypes = _figuresDictionary.Values.ToArray();
    }

    public IFigureTypeInfo[] FigureTypes { get; }

    /// <inheritdoc />
    public IFigureTypeInfo GetFigureTypeById(Figure uniqueUnit)
    {
        return _figuresDictionary[uniqueUnit];
    }
}