using BattleChess3.Core.Model;

namespace BattleChess3.DoubleChessFigures;

public class SingleBishop : IDoubleChessFigureTypeWithChainedAttackMoves
{
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 1),
        (1, -1), (1, 1)
    };
}