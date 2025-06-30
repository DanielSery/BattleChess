using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.LordOfTheRingsFigures;

public class PipinTroll : ILordOfTheRingsFigureType, IFigureTypeWithDifferentAttacksAndMoves
{
    int IFigureType.FigureId => 7;
    
    Position[] IFigureTypeWithDifferentAttacksAndMoves.MovePositions { get; } =
    [
        new(-1, 0), new(1, 0), new(0, -1), new(0, 1)
    ];

    Position[] IFigureTypeWithDifferentAttacksAndMoves.AttackPositions { get; } =
    [
        new(-2, 0),
        new(-1, -1), new(-1, 1),
        new(0, -2), new(0, 2),
        new(1, -1), new(1, 1),
        new(2, 0)
    ];
}