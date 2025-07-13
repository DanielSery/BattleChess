using System.Collections;
using BattleChess3.Maps;

namespace BattleChess3.Multiplayer.Utilities;

public static class UnlockedFiguresHelper
{
    public static bool IsValid(this MapBlueprint mapBlueprint, byte[] unlockedFigures)
    {
        var bitArray = new BitArray(unlockedFigures);
        return mapBlueprint.Figures.All(x => bitArray[x.FigureId]);
    }
}