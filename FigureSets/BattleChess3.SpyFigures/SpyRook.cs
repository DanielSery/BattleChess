using BattleChess3.Core.Model;

namespace BattleChess3.SpyFigures;

public class SpyRook : ISpyFigureType, ISpyFigureTypeWithChainedAttackMoves
{
    Position[] ISpyFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } = 
    {
        (-1, 0), (1, 0), (0, -1), (0, 1),
    };
}