using CrownsGuard.Core.Figures;
using CrownsGuard.Core.SimulatedBoard;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.FigureDefinitions.Localization;

namespace CrownsGuard.FigureDefinitions;

public sealed class CrownsGuardFigureGroup : IFigureGroup
{
    private readonly Dictionary<FigureId, IFigureType> _figuresDictionary;
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrownsGuardFigureGroup)}_Name"];

    public CrownsGuardFigureGroup()
    {
        _figuresDictionary = new Dictionary<FigureId, IFigureType>()
        {
            { FigureId.Empty, new Empty() },
            { FigureId.Wall, new Wall() },
            { FigureId.Explosives, new Explosives() },
            { FigureId.Trench, new Trench() },
            { FigureId.Fire, new Fire() },

            { FigureId.Peasant, new Peasant() },
            { FigureId.Spearman, new Spearman() },
            { FigureId.Pikeman, new Pikeman() },
            { FigureId.LegionarySword, new LegionarySword() },
            { FigureId.LegionaryPike, new LegionaryPike() },

            { FigureId.MountedKnight, new MountedKnight() },
            { FigureId.CamelRider, new CamelRider() },
            { FigureId.MountedArcher, new MountedArcher() },
            { FigureId.CamelArcher, new CamelArcher() },
            { FigureId.Scout, new Scout() },
            { FigureId.Dogs, new Dogs() },
            { FigureId.Queen, new Queen() },

            { FigureId.Knight, new Knight() },
            { FigureId.Samurai, new Samurai() },
            { FigureId.Chinese, new Chinese() },
            { FigureId.Nordguard, new Nordguard() },
            { FigureId.Blade, new Blade() },
            { FigureId.Elephant, new Elephant() },

            { FigureId.Archer, new Archer() },
            { FigureId.JapanArcher, new JapanArcher() },
            { FigureId.Ranger, new Ranger() },
            { FigureId.Crossbow, new Crossbow() },
            { FigureId.Musketeer, new Musketeer() },
            { FigureId.Cannon, new Cannon() },
            { FigureId.Catapult, new Catapult() },

            { FigureId.Spartan, new Spartan() },
            { FigureId.Warhammer, new Warhammer() },
            { FigureId.BattleAxe, new BattleAxe() },
            { FigureId.Mage, new Mage() },
            { FigureId.Wizzard, new Wizzard() },

            { FigureId.King, new King() },
            { FigureId.Trader, new Trader() },
            { FigureId.Bard, new Bard() },
            { FigureId.Barbarian, new Barbarian() },
            { FigureId.Whiplash, new Whiplash() },
            { FigureId.Priest, new Priest() },

            { FigureId.Builder, new Builder() },
            { FigureId.Alchemist, new Alchemist() },
            { FigureId.Miner, new Miner() },
            { FigureId.Dragon, new Dragon() },
        };
        FigureTypes = _figuresDictionary.Values.ToArray();
    }

    public IFigureType[] FigureTypes { get; }

    /// <inheritdoc />
    public IFigureType GetFigureTypeById(FigureId uniqueUnitId)
    {
        return _figuresDictionary[uniqueUnitId];
    }
}