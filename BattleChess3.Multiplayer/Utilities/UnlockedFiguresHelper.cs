using System.Collections;
using BattleChess3.Game.GameBoard;

namespace BattleChess3.Multiplayer.Utilities;

public static class UnlockedFiguresHelper
{
    public static bool IsValid(this BoardBlueprint boardBlueprint, byte[] unlockedFigures)
    {
        var bitArray = new BitArray(unlockedFigures);
        return boardBlueprint.Figures.All(x => bitArray[x.FigureId]);
    }
}