using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.FigureDefinitions.Utilities;

public static class FigureValueHelper
{
    private static readonly int[] FigureValues;
    
    static FigureValueHelper()
    {
        var maxValue = Enum.GetValues<Figure>().Select(x => (int)x).Max();
        
        FigureValues = new int[maxValue + 1];
        FigureValues[(int)Figure.Empty] = 0;
        FigureValues[(int)Figure.Alchemist] = 4;
        FigureValues[(int)Figure.Archer] = 10;
        FigureValues[(int)Figure.Barbarian] = 4;
        FigureValues[(int)Figure.Bard] = 12;
        FigureValues[(int)Figure.BattleAxe] = 6;
        FigureValues[(int)Figure.Blade] = 16;
        FigureValues[(int)Figure.Builder] = 4;
        FigureValues[(int)Figure.CamelArcher] = 8;
        FigureValues[(int)Figure.CamelRider] = 6;
        FigureValues[(int)Figure.Cannon] = 12;
        FigureValues[(int)Figure.Catapult] = 12;
        FigureValues[(int)Figure.Chinese] = 8;
        FigureValues[(int)Figure.Crossbow] = 10;
        FigureValues[(int)Figure.Dogs] = 10;
        FigureValues[(int)Figure.Dragon] = 12;
        FigureValues[(int)Figure.Elephant] = 10;
        FigureValues[(int)Figure.Explosives] = 1;
        FigureValues[(int)Figure.Fire] = 0;
        FigureValues[(int)Figure.JapanArcher] = 10;
        FigureValues[(int)Figure.King] = 5;
        FigureValues[(int)Figure.Knight] = 10;
        FigureValues[(int)Figure.LegionaryPike] = 4;
        FigureValues[(int)Figure.LegionarySword] = 2;
        FigureValues[(int)Figure.Mage] = 16;
        FigureValues[(int)Figure.Miner] = 3;
        FigureValues[(int)Figure.MountedArcher] = 10;
        FigureValues[(int)Figure.MountedKnight] = 10;
        FigureValues[(int)Figure.Musketeer] = 12;
        FigureValues[(int)Figure.Ninja] = 3;
        FigureValues[(int)Figure.Nordguard] = 8;
        FigureValues[(int)Figure.Peasant] = 2;
        FigureValues[(int)Figure.Pikeman] = 3;
        FigureValues[(int)Figure.Priest] = 12;
        FigureValues[(int)Figure.Queen] = 18;
        FigureValues[(int)Figure.Ranger] = 10;
        FigureValues[(int)Figure.Samurai] = 6;
        FigureValues[(int)Figure.Scout] = 10;
        FigureValues[(int)Figure.Spartan] = 4;
        FigureValues[(int)Figure.Spearman] = 3;
        FigureValues[(int)Figure.Trader] = 7;
        FigureValues[(int)Figure.Trench] = 0;
        FigureValues[(int)Figure.Wall] = 1;
        FigureValues[(int)Figure.Warhammer] = 6;
        FigureValues[(int)Figure.Whiplash] = 6;
        FigureValues[(int)Figure.Wizzard] = 16;
    }
    
    public static int GetFigureValue(this Figure figure)
    {
        return FigureValues[(int)figure.GetFigureType()];
    }
}