using CrownsGuard.Core.Figures;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureValueHelper
{
    private static readonly int[] FigureValues;
    
    static FigureValueHelper()
    {
        var maxValue = Enum.GetValues<FigureId>().Select(x => (int)x).Max();
        
        FigureValues = new int[maxValue + 1];
        FigureValues[(int)FigureId.Empty] = 0;
        FigureValues[(int)FigureId.Alchemist] = 4;
        FigureValues[(int)FigureId.Archer] = 10;
        FigureValues[(int)FigureId.Barbarian] = 4;
        FigureValues[(int)FigureId.Bard] = 12;
        FigureValues[(int)FigureId.BattleAxe] = 6;
        FigureValues[(int)FigureId.Blade] = 16;
        FigureValues[(int)FigureId.Builder] = 4;
        FigureValues[(int)FigureId.CamelArcher] = 8;
        FigureValues[(int)FigureId.CamelRider] = 6;
        FigureValues[(int)FigureId.Cannon] = 12;
        FigureValues[(int)FigureId.Catapult] = 12;
        FigureValues[(int)FigureId.Chinese] = 8;
        FigureValues[(int)FigureId.Crossbow] = 10;
        FigureValues[(int)FigureId.Dogs] = 10;
        FigureValues[(int)FigureId.Dragon] = 12;
        FigureValues[(int)FigureId.Elephant] = 10;
        FigureValues[(int)FigureId.Explosives] = 1;
        FigureValues[(int)FigureId.Fire] = 0;
        FigureValues[(int)FigureId.JapanArcher] = 10;
        FigureValues[(int)FigureId.King] = 5;
        FigureValues[(int)FigureId.Knight] = 10;
        FigureValues[(int)FigureId.LegionaryPike] = 4;
        FigureValues[(int)FigureId.LegionarySword] = 2;
        FigureValues[(int)FigureId.Mage] = 16;
        FigureValues[(int)FigureId.Miner] = 3;
        FigureValues[(int)FigureId.MountedArcher] = 10;
        FigureValues[(int)FigureId.MountedKnight] = 10;
        FigureValues[(int)FigureId.Musketeer] = 12;
        FigureValues[(int)FigureId.Ninja] = 3;
        FigureValues[(int)FigureId.Nordguard] = 8;
        FigureValues[(int)FigureId.Peasant] = 2;
        FigureValues[(int)FigureId.Pikeman] = 3;
        FigureValues[(int)FigureId.Priest] = 12;
        FigureValues[(int)FigureId.Queen] = 18;
        FigureValues[(int)FigureId.Ranger] = 10;
        FigureValues[(int)FigureId.Samurai] = 6;
        FigureValues[(int)FigureId.Scout] = 10;
        FigureValues[(int)FigureId.Spartan] = 4;
        FigureValues[(int)FigureId.Spearman] = 3;
        FigureValues[(int)FigureId.Trader] = 7;
        FigureValues[(int)FigureId.Trench] = 0;
        FigureValues[(int)FigureId.Wall] = 1;
        FigureValues[(int)FigureId.Warhammer] = 6;
        FigureValues[(int)FigureId.Whiplash] = 6;
        FigureValues[(int)FigureId.Wizzard] = 16;
    }
    
    public static int GetFigureValue(this FigureId figureId)
    {
        return FigureValues[(int)figureId];
    }
}