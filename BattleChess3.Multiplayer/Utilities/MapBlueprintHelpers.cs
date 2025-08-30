using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Helpers;

namespace BattleChess3.Multiplayer.Utilities;

public static class MapBlueprintHelpers
{
    public static byte[] GetByteData(this MapBlueprint map)
    {
        var myMapData = new byte[32];
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var index = i * 2;
            myMapData[index] = (byte)(map.Figures[i].Player.ToInt() + (map.Figures[i].IsKing ? 128 : 0));
            myMapData[index + 1] = (byte)(map.Figures[i].FigureId);
        }

        return myMapData;
    }
}