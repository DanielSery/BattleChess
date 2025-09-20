using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.FigureDefinitions.Localization;
using System.Collections.Frozen;

namespace CrownsGuard.FigureDefinitions;

public sealed class CrownsGuardFigureTypeInfoGroup : IFigureTypeInfoGroup
{
    private readonly FrozenDictionary<Figure, IFigureTypeInfo> _figuresDictionary;
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(CrownsGuardFigureTypeInfoGroup)}_Name"];

    public static readonly IFigureTypeInfo Empty = new NeutralFigureTypeInfo(nameof(Empty), Figure.Empty);
    public static readonly IFigureTypeInfo Wall = new NeutralFigureTypeInfo(nameof(Wall), Figure.Wall);
    public static readonly IFigureTypeInfo Explosives = new NeutralFigureTypeInfo(nameof(Explosives), Figure.Explosives);
    public static readonly IFigureTypeInfo Trench = new NeutralFigureTypeInfo(nameof(Trench), Figure.Trench);
    public static readonly IFigureTypeInfo Fire = new NeutralFigureTypeInfo(nameof(Fire), Figure.Fire);

    public static readonly IFigureTypeInfo Peasant = new PlayerFigureTypeInfo(nameof(Peasant), Figure.Peasant);
    public static readonly IFigureTypeInfo Spearman = new PlayerFigureTypeInfo(nameof(Spearman), Figure.Spearman);
    public static readonly IFigureTypeInfo Pikeman = new PlayerFigureTypeInfo(nameof(Pikeman), Figure.Pikeman);
    public static readonly IFigureTypeInfo LegionarySword = new PlayerFigureTypeInfo(nameof(LegionarySword), Figure.LegionarySword);
    public static readonly IFigureTypeInfo LegionaryPike = new PlayerFigureTypeInfo(nameof(LegionaryPike), Figure.LegionaryPike);

    public static readonly IFigureTypeInfo MountedKnight = new PlayerFigureTypeInfo(nameof(MountedKnight), Figure.MountedKnight);
    public static readonly IFigureTypeInfo CamelRider = new PlayerFigureTypeInfo(nameof(CamelRider), Figure.CamelRider);
    public static readonly IFigureTypeInfo MountedArcher = new PlayerFigureTypeInfo(nameof(MountedArcher), Figure.MountedArcher);
    public static readonly IFigureTypeInfo CamelArcher = new PlayerFigureTypeInfo(nameof(CamelArcher), Figure.CamelArcher);
    public static readonly IFigureTypeInfo Scout = new PlayerFigureTypeInfo(nameof(Scout), Figure.Scout);
    public static readonly IFigureTypeInfo Dogs = new PlayerFigureTypeInfo(nameof(Dogs), Figure.Dogs);
    public static readonly IFigureTypeInfo Queen = new PlayerFigureTypeInfo(nameof(Queen), Figure.Queen);

    public static readonly IFigureTypeInfo Knight = new PlayerFigureTypeInfo(nameof(Knight), Figure.Knight);
    public static readonly IFigureTypeInfo Samurai = new PlayerFigureTypeInfo(nameof(Samurai), Figure.Samurai);
    public static readonly IFigureTypeInfo Chinese = new PlayerFigureTypeInfo(nameof(Chinese), Figure.Chinese);
    public static readonly IFigureTypeInfo Nordguard = new PlayerFigureTypeInfo(nameof(Nordguard), Figure.Nordguard);
    public static readonly IFigureTypeInfo Blade = new PlayerFigureTypeInfo(nameof(Blade), Figure.Blade);
    public static readonly IFigureTypeInfo Elephant = new PlayerFigureTypeInfo(nameof(Elephant), Figure.Elephant);

    public static readonly IFigureTypeInfo Archer = new PlayerFigureTypeInfo(nameof(Archer), Figure.Archer);
    public static readonly IFigureTypeInfo JapanArcher = new PlayerFigureTypeInfo(nameof(JapanArcher), Figure.JapanArcher);
    public static readonly IFigureTypeInfo Ranger = new PlayerFigureTypeInfo(nameof(Ranger), Figure.Ranger);
    public static readonly IFigureTypeInfo Crossbow = new PlayerFigureTypeInfo(nameof(Crossbow), Figure.Crossbow);
    public static readonly IFigureTypeInfo Musketeer = new PlayerFigureTypeInfo(nameof(Musketeer), Figure.Musketeer);
    public static readonly IFigureTypeInfo Cannon = new PlayerFigureTypeInfo(nameof(Cannon), Figure.Cannon);
    public static readonly IFigureTypeInfo Catapult = new PlayerFigureTypeInfo(nameof(Catapult), Figure.Catapult);

    public static readonly IFigureTypeInfo Spartan = new PlayerFigureTypeInfo(nameof(Spartan), Figure.Spartan);
    public static readonly IFigureTypeInfo Warhammer = new PlayerFigureTypeInfo(nameof(Warhammer), Figure.Warhammer);
    public static readonly IFigureTypeInfo BattleAxe = new PlayerFigureTypeInfo(nameof(BattleAxe), Figure.BattleAxe);
    public static readonly IFigureTypeInfo Mage = new PlayerFigureTypeInfo(nameof(Mage), Figure.Mage);
    public static readonly IFigureTypeInfo Wizzard = new PlayerFigureTypeInfo(nameof(Wizzard), Figure.Wizzard);

    public static readonly IFigureTypeInfo King = new PlayerFigureTypeInfo(nameof(King), Figure.King);
    public static readonly IFigureTypeInfo Trader = new PlayerFigureTypeInfo(nameof(Trader), Figure.Trader);
    public static readonly IFigureTypeInfo Bard = new PlayerFigureTypeInfo(nameof(Bard), Figure.Bard);
    public static readonly IFigureTypeInfo Barbarian = new PlayerFigureTypeInfo(nameof(Barbarian), Figure.Barbarian);
    public static readonly IFigureTypeInfo Whiplash = new PlayerFigureTypeInfo(nameof(Whiplash), Figure.Whiplash);
    public static readonly IFigureTypeInfo Priest = new PlayerFigureTypeInfo(nameof(Priest), Figure.Priest);

    public static readonly IFigureTypeInfo Builder = new PlayerFigureTypeInfo(nameof(Builder), Figure.Builder);
    public static readonly IFigureTypeInfo Alchemist = new PlayerFigureTypeInfo(nameof(Alchemist), Figure.Alchemist);
    public static readonly IFigureTypeInfo Miner = new PlayerFigureTypeInfo(nameof(Miner), Figure.Miner);
    public static readonly IFigureTypeInfo Dragon = new PlayerFigureTypeInfo(nameof(Dragon), Figure.Dragon);

    public CrownsGuardFigureTypeInfoGroup()
    {
        FigureTypes = [
            Empty, Wall, Explosives, Trench, Fire,

            Peasant, Spearman, Pikeman, LegionarySword, LegionaryPike,

            MountedKnight, CamelRider, MountedArcher, CamelArcher, Scout, Dogs, Queen,

            Knight, Samurai, Chinese, Nordguard, Blade, Elephant,

            Archer, JapanArcher, Ranger, Crossbow, Musketeer, Cannon, Catapult,

            Spartan, Warhammer, BattleAxe, Mage, Wizzard,

            King, Trader, Bard, Barbarian, Whiplash, Priest,

            Builder, Alchemist, Miner, Dragon

        ];

        _figuresDictionary = FigureTypes.ToFrozenDictionary(x => x.Figure, x => x);
    }

    public IFigureTypeInfo[] FigureTypes { get; }

    /// <inheritdoc />
    public IFigureTypeInfo GetFigureTypeById(Figure uniqueUnit)
    {
        return _figuresDictionary[uniqueUnit];
    }
}