using BattleChess3.Core.Model;

namespace BattleChess3.SpyFigures;

public class SpyQueen : ISpyFigureType, ISpyFigureTypeWithChainedAttackMoves
{
    Position[] ISpyFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };
}