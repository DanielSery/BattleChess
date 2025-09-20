using CrownsGuard.Core.Figures;

namespace CrownsGuard.Multiplayer.Utilities;

public static class MapBlueprintHelpers
{
    public static int[] GetIntData(this Figure[] map)
    {
        var myMapData = new int[16];

        for (var i = 0; i < map.Length; i++)
        {
            myMapData[i] = (int)map[i];
        }

        return myMapData;
    }
}