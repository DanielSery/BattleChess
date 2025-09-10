using CrownsGuard.Core.Figures;
using CrownsGuard.FigureDefinitions.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureValueHelper
{
    public static int GetFigureValue(this FigureId figureId)
    {
        return figureId switch
        {
            FigureId.Empty => 0,
            FigureId.Alchemist => 4,
            FigureId.Archer => 10,
            FigureId.Barbarian => 4,
            FigureId.Bard => 12,
            FigureId.BattleAxe => 6,
            FigureId.Blade => 16,
            FigureId.Builder => 4,
            FigureId.CamelArcher => 8,
            FigureId.CamelRider => 6,
            FigureId.Cannon => 12,
            FigureId.Catapult => 12,
            FigureId.Chinese => 8,
            FigureId.Crossbow => 10,
            FigureId.Dogs => 10,
            FigureId.Dragon => 12,
            FigureId.Elephant => 10,
            FigureId.Explosives => 1,
            FigureId.Fire => 0,
            FigureId.JapanArcher => 10,
            FigureId.King => 5,
            FigureId.Knight => 10,
            FigureId.LegionaryPike => 4,
            FigureId.LegionarySword => 2,
            FigureId.Mage => 16,
            FigureId.Miner => 3,
            FigureId.MountedArcher => 10,
            FigureId.MountedKnight => 10,
            FigureId.Musketeer => 12,
            FigureId.Ninja => 3,
            FigureId.Nordguard => 8,
            FigureId.Peasant => 2,
            FigureId.Pikeman => 3,
            FigureId.Priest => 12,
            FigureId.Queen => 18,
            FigureId.Ranger => 10,
            FigureId.Samurai => 6,
            FigureId.Scout => 10,
            FigureId.Spartan => 4,
            FigureId.Spearman => 3,
            FigureId.Trader => 7,
            FigureId.Trench => 0,
            FigureId.Wall => 1,
            FigureId.Warhammer => 6,
            FigureId.Whiplash => 6,
            FigureId.Wizzard => 16,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}