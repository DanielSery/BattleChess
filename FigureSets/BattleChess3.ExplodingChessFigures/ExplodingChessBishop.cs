using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessBishop : IExplodingChessFigureType, IExplodingChessFigureTypeWithChainedAttackMoves
{
    int IFigureType.FigureId => 1;
    
    Position[] IExplodingChessFigureTypeWithChainedAttackMoves.AttackMoveDirections { get; } =
    [
        new(-1, -1), new(-1, 1),
        new(1, -1), new(1, 1)
    ];
}