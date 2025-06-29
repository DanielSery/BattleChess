using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessKnight : IExplodingChessFigureType, IExplodingChessFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IExplodingChessFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } = 
    {
        (-2, -1), (-2, 1),
        (-1, -2), (-1, 2),
        (1, -2), (1, 2),
        (2, -1), (2, 1)
    };
}