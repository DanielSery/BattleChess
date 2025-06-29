using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DoubleChessFigures;

public class SingleKnight : IDoubleChessFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IDoubleChessFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } = 
    {
        (-2, -1), (-2, 1),
        (-1, -2), (-1, 2),
        (1, -2), (1, 2),
        (2, -1), (2, 1)
    };
}