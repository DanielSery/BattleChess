﻿using BattleChess3.Core.Model;

namespace BattleChess3.DoubleChessFigures;

public class SingleRook : IDoubleChessFigureTypeWithChainedAttackMoves
{
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } = 
    {
        (-1, 0), (1, 0), (0, -1), (0, 1),
    };
}