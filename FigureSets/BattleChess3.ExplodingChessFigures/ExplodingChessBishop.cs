using BattleChess3.Core.Model;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessBishop : IExplodingChessFigureType, IExplodingChessFigureTypeWithChainedAttackMoves
{
    Position[] IExplodingChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 1),
        (1, -1), (1, 1),
    };
}