using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ExplodingChessFigures;

public class ExplodingChessKnight : IExplodingChessFigureType, IExplodingChessFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IExplodingChessFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}