using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using System.Collections;

namespace CrownsGuard.Multiplayer.Utilities;

public static class UnlockedFiguresHelper
{
    public static bool IsValid(this Figure[] boardBlueprint, byte[] unlockedFigures)
    {
        var bitArray = new BitArray(unlockedFigures);
        var kingCount = 0;
        
        foreach (var x in boardBlueprint)
        {
            if (x.IsKing()) kingCount++;
            if (x.GetFigureColor() == Figure.IsBlack) return false;
            if (!bitArray[(int)x.GetFigureType()]) return false;
        }

        return kingCount == 1 && boardBlueprint.Length == 16;
    }
}