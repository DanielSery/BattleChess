﻿using BattleChess3.Core.Model;

namespace BattleChess3.DoubleChessFigures;

public class SingleQueen : IDoubleChessFigureTypeWithChainedAttackMoves
{
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };
}