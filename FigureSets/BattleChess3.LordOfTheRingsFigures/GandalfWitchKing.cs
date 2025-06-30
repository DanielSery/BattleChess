using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.LordOfTheRingsFigures;

public class GandalfWitchKing : ILordOfTheRingsFigureType, IFigureTypeWithDifferentAttacksAndMoves
{
    int IFigureType.FigureId => 3;
    
    Position[] IFigureTypeWithDifferentAttacksAndMoves.MovePositions { get; } =
    [
        new(-4, -2), new(-4, 0), new(-4, 2),
        new(-3, -3), new(-3, -1), new(-3, 1), new(-3, 3),
        new(-2, -4), new(-2, 0), new(-2, 4),
        new(-1, -3), new(-1, -1), new(-1, 1), new(-1, 3),
        new(0, -4), new(0, -2), new(0, 2), new(0, 4),
        new(1, -3), new(1, -1), new(1, 1), new(1, 3),
        new(2, -4), new(2, 0), new(2, 4),
        new(3, -3), new(3, -1), new(3, 1), new(3, 3),
        new(4, -2), new(4, 0), new(4, 2)
    ];

    Position[] IFigureTypeWithDifferentAttacksAndMoves.AttackPositions { get; } =
    [
        new(-2, -1), new(-2, 1),
        new(-1, -2), new(-1, 2),
        new(1, -2), new(1, 2),
        new(2, -1), new(2, 1)
    ];
}