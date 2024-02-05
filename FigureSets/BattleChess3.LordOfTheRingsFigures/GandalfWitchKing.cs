﻿using BattleChess3.DefaultFigures;

namespace BattleChess3.LordOfTheRingsFigures;

public class GandalfWitchKing : ILordOfTheRingsFigureType, IFigureTypeWithDifferentMoves
{
    public static readonly GandalfWitchKing Instance = new();

    int[] IFigureTypeWithDifferentMoves.Actions { get; } = {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 0, 0, 3, 1, 3, 0, 0, 1, 0, 0, 0,
        0, 0, 0, 0, 1, 3, 1, 0, 1, 3, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 0, 1, 0, 8, 0, 1, 0, 1, 0, 0, 0,
        0, 0, 0, 0, 1, 3, 1, 0, 1, 3, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 0, 0, 3, 1, 3, 0, 0, 1, 0, 0, 0,
        0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
    };
}
