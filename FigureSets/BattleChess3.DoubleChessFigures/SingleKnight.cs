using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DoubleChessFigures;

public class SingleKnight : IDoubleChessFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IDoubleChessFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}