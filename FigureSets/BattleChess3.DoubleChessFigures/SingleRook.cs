using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DoubleChessFigures;

public class SingleRook : IDoubleChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 6;
    
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } = 
    {
        (-1, 0), (1, 0), (0, -1), (0, 1),
    };
}