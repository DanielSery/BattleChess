using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessBishop : IExplodingChessFigureType, IExplodingChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 1;
    
    Position[] IExplodingChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        (-1, -1), (-1, 1),
        (1, -1), (1, 1)
    ];
}