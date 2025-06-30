using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.DoubleChessFigures;

public class SingleBishop : IDoubleChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 1;
    
    Position[] IDoubleChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];
}