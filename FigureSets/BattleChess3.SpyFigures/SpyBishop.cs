using BattleChess3.Core.Model;

namespace BattleChess3.SpyFigures;

public class SpyBishop : ISpyFigureType, ISpyFigureTypeWithChainedAttackMoves
{
    Position[] ISpyFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 1),
        (1, -1), (1, 1),
    };
}