using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Multiplayer.Utilities;

public static class MapBlueprintHelpers
{
    public static int[] GetIntData(this BoardBlueprint map)
    {
        var myMapData = new int[16];

        for (var i = 0; i < map.Figures.Length; i++)
        {
            myMapData[i] = map.Figures[i].ToInt();
        }

        return myMapData;
    }
}