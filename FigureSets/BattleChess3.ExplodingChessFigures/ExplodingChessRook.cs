using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessRook : IExplodingChessFigureType, IExplodingChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 6;
    
    Position[] IExplodingChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        (-1, 0), (1, 0), (0, -1), (0, 1)
    ];
}