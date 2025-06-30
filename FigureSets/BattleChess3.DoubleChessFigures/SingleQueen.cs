using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DoubleChessFigures;

public class SingleQueen : IDoubleChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 5;
    
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];
}