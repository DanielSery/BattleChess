using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DoubleChessFigures;

public class SingleRook : IDoubleChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 6;
    
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];
}