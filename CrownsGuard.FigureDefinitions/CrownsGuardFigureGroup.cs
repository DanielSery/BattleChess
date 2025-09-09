using CrownsGuard.Core.Figures;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions;

public sealed class CrownsGuardFigureGroup : IFigureGroup
{
    private readonly Dictionary<FigureType, IFigureType> _figuresDictionary;
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrownsGuardFigureGroup)}_Name"];

    public CrownsGuardFigureGroup()
    {
        _figuresDictionary = new Dictionary<FigureType, IFigureType>()
        {
            { FigureType.Empty, new Empty() },
            { FigureType.Wall, new Wall() },
            { FigureType.Explosives, new Explosives() },
            { FigureType.Trench, new Trench() },
            { FigureType.Fire, new Fire() },

            { FigureType.Peasant, new Peasant() },
            { FigureType.Spearman, new Spearman() },
            { FigureType.Pikeman, new Pikeman() },
            { FigureType.LegionarySword, new LegionarySword() },
            { FigureType.LegionaryPike, new LegionaryPike() },

            { FigureType.MountedKnight, new MountedKnight() },
            { FigureType.CamelRider, new CamelRider() },
            { FigureType.MountedArcher, new MountedArcher() },
            { FigureType.CamelArcher, new CamelArcher() },
            { FigureType.Scout, new Scout() },
            { FigureType.Dogs, new Dogs() },
            { FigureType.Queen, new Queen() },

            { FigureType.Knight, new Knight() },
            { FigureType.Samurai, new Samurai() },
            { FigureType.Chinese, new Chinese() },
            { FigureType.Nordguard, new Nordguard() },
            { FigureType.Blade, new Blade() },
            { FigureType.Elephant, new Elephant() },

            { FigureType.Archer, new Archer() },
            { FigureType.JapanArcher, new JapanArcher() },
            { FigureType.Ranger, new Ranger() },
            { FigureType.Crossbow, new Crossbow() },
            { FigureType.Musketeer, new Musketeer() },
            { FigureType.Cannon, new Cannon() },
            { FigureType.Catapult, new Catapult() },

            { FigureType.Spartan, new Spartan() },
            { FigureType.Warhammer, new Warhammer() },
            { FigureType.BattleAxe, new BattleAxe() },
            { FigureType.Mage, new Mage() },
            { FigureType.Wizzard, new Wizzard() },

            { FigureType.King, new King() },
            { FigureType.Trader, new Trader() },
            { FigureType.Bard, new Bard() },
            { FigureType.Barbarian, new Barbarian() },
            { FigureType.Whiplash, new Whiplash() },
            { FigureType.Priest, new Priest() },

            { FigureType.Builder, new Builder() },
            { FigureType.Alchemist, new Alchemist() },
            { FigureType.Miner, new Miner() },
            { FigureType.Dragon, new Dragon() },
        };
        FigureTypes = _figuresDictionary.Values.ToArray();
    }

    public IFigureType[] FigureTypes { get; }

    /// <inheritdoc />
    public IFigureType GetFigureTypeById(int uniqueUnitId)
    {
        return _figuresDictionary[(FigureType)uniqueUnitId];
    }
}