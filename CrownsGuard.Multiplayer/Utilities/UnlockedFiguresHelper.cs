using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using System.Collections;

namespace CrownsGuard.Multiplayer.Utilities;

public static class UnlockedFiguresHelper
{
    public static bool IsValid(this Figure[] boardBlueprint, byte[] unlockedFigures)
    {
        var bitArray = new BitArray(unlockedFigures);
        return boardBlueprint.All(x => bitArray[(int)x.GetFigureType()]);
    }
}