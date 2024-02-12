﻿using BattleChess3.Core.Model;

namespace BattleChess3.LordOfTheRingsFigures;

public class GimliNazgul : ILordOfTheRingsFigureType, IFigureTypeWithChainedAttacksAndMoves
{
    Position[] IFigureTypeWithChainedAttacksAndMoves.MoveDirections { get; } =
    {
        (-1, -1), (-1, 1),
        (1, -1), (1, 1)
    };

    Position[] IFigureTypeWithChainedAttacksAndMoves.AttackDirections { get; } =
    {
        (-1, 0),
        (1, 0),
        (0, -1),
        (0, 1)
    };
}