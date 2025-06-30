using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.ChessFigures;

public class Knight : IChessFigureType, IFigureTypeWithDifferentAttackMoves
{
    int IFigureType.FigureId => 2;
    
    Position[] IFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}