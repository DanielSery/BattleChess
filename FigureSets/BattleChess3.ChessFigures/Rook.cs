using BattleChess3.Core.Model;

namespace BattleChess3.ChessFigures;

public class Rook : IChessFigureType, IFigureTypeWithChainedAttackMoves
{
    Position[] IFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } = 
    {
        (-1, 0), (1, 0), (0, -1), (0, 1),
    };
}