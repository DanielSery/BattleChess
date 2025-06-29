using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessQueen : IExplodingChessFigureType, IExplodingChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 5;
    
    Position[] IExplodingChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    {
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };
}