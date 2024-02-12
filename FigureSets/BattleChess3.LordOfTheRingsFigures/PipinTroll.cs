﻿using BattleChess3.Core.Model;

namespace BattleChess3.LordOfTheRingsFigures;

public class PipinTroll : ILordOfTheRingsFigureType, IFigureTypeWithDifferentAttacksAndMoves
{
    Position[] IFigureTypeWithDifferentAttacksAndMoves.MovePositions { get; } =
    {
        (-1, 0), (1, 0), (0, -1), (0, 1)
    };

    Position[] IFigureTypeWithDifferentAttacksAndMoves.AttackPositions { get; } =
    {
        (-2, 0),
        (-1, -1), (-1, 1),
        (0, -2), (0, 2),
        (1, -1), (1, 1),
        (2, 0)
    };
}