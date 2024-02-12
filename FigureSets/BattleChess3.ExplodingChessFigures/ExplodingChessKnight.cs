using BattleChess3.Core.Model;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessKnight : IExplodingChessFigureType, IExplodingChessFigureTypeWithDifferentAttackMoves
{
    Position[] IExplodingChessFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } = 
    {
        (-2, -1), (-2, 1),
        (-1, -2), (-1, 2),
        (1, -2), (1, 2),
        (2, -1), (2, 1)
    };
}