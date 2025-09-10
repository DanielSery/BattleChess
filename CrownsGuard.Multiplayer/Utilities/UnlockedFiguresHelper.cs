using System.Collections;
using CrownsGuard.Core.GameBoard;

namespace CrownsGuard.Multiplayer.Utilities;

public static class UnlockedFiguresHelper
{
    public static bool IsValid(this BoardBlueprint boardBlueprint, byte[] unlockedFigures)
    {
        var bitArray = new BitArray(unlockedFigures);
        return boardBlueprint.Figures.All(x => bitArray[(int)x.FigureType]);
    }
}