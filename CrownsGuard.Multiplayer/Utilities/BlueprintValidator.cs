using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using System.Collections;

namespace CrownsGuard.Multiplayer.Utilities;

public static class BlueprintValidator
{
    public static bool IsValid(this Figure[] boardBlueprint, IReadOnlyList<byte> unlockedFigures)
    {
        var bitArray = new BitArray(unlockedFigures.ToArray());
        var kingCount = 0;
        var totalValue = 0;
        
        foreach (var x in boardBlueprint)
        {
            if (x.IsKing())
            {
                if (!x.IsWhite()) return false;
                kingCount++;
            }
            
            if (x.IsNeutralFigure() && !x.IsNeutral()) return false;
            if (!x.IsNeutralFigure() && x.GetFigureColor() != Figure.IsWhite) return false;
            
            if (!bitArray[(int)x.GetFigureType()]) return false;
            totalValue += x.GetFigureValue();
        }

        return kingCount == 1 && 
               boardBlueprint.Length == 16 &&
               totalValue <= Constants.MaxMapsPoints;
    }
}