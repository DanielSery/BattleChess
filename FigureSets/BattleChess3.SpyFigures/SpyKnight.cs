using BattleChess3.Core.Model;

namespace BattleChess3.SpyFigures;

public class SpyKnight : ISpyFigureType, ISpyFigureTypeWithDifferentAttackMoves
{
    Position[] ISpyFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } = 
    {
        (-2, -1), (-2, 1),
        (-1, -2), (-1, 2),
        (1, -2), (1, 2),
        (2, -1), (2, 1)
    };
}