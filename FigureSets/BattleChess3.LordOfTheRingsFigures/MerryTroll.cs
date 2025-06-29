using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.LordOfTheRingsFigures;

public class MerryTroll : ILordOfTheRingsFigureType, IFigureTypeWithDifferentAttacksAndMoves
{
    int IFigureType.FigureId => 6;
    
    Position[] IFigureTypeWithDifferentAttacksAndMoves.MovePositions { get; } =
    {
        (-1, 0), (1, 0), (0, -1), (0, 1)
    };

    Position[] IFigureTypeWithDifferentAttacksAndMoves.AttackPositions { get; } =
    {
        (-2, 0),
        (-1, -1), (-1, 1),
        (0, -2), (0, 2),
        (1, -1), (1, 1),
        (2, 0)
    };
}