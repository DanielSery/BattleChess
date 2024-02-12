﻿using BattleChess3.Core.Model;

namespace BattleChess3.ChessFigures;

public class Knight : IChessFigureType, IFigureTypeWithDifferentAttackMoves
{
    Position[] IFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } = 
    {
        (-2, -1), (-2, 1),
        (-1, -2), (-1, 2),
        (1, -2), (1, 2),
        (2, -1), (2, 1)
    };
}