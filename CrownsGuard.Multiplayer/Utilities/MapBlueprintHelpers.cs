using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;

namespace CrownsGuard.Multiplayer.Utilities;

public static class MapBlueprintHelpers
{
    public static byte[] GetByteData(this BoardBlueprint map)
    {
        var myMapData = new byte[32];
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var index = i * 2;
            myMapData[index] = (byte)(map.Figures[i].Player.ToInt() + (map.Figures[i].IsKing ? 128 : 0));
            myMapData[index + 1] = (byte)(map.Figures[i].FigureType);
        }

        return myMapData;
    }
}