using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessQueen : IExplodingChessFigureType, IExplodingChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 5;
    
    Position[] IExplodingChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, -1), new(-1, 0), new(-1, 1),
        new(0, -1), new(0, 1),
        new(1, -1), new(1, 0), new(1, 1)
    ];
}