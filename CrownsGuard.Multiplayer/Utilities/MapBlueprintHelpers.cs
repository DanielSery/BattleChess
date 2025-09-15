using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Multiplayer.Utilities;

public static class MapBlueprintHelpers
{
    public static int[] GetIntData(this BoardBlueprint map)
    {
        var myMapData = new int[16];

        for (var i = 0; i < map.Figures.Length; i++)
        {
            myMapData[i] = (int)map.Figures[i];
        }

        return myMapData;
    }
}