using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.CrossFireFigures;

public class Viking : IFigureTypeWithDifferentAttackMoves, ICrossFireFigureType
{
    int IFigureType.FigureId => 25;
    
    Position[] IFigureTypeWithDifferentAttackMoves.AttackMovePositions { get; } =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}