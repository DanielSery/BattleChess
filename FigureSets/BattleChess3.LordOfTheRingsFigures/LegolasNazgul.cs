using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.LordOfTheRingsFigures;

public class LegolasNazgul : ILordOfTheRingsFigureType, IFigureTypeWithChainedAttacksAndMoves
{
    int IFigureType.FigureId => 5;
    
    Position[] IFigureTypeWithChainedAttacksAndMoves.MoveDirections { get; } =
    {
        (-1, -1), (-1, 1),
        (1, -1), (1, 1)
    };

    Position[] IFigureTypeWithChainedAttacksAndMoves.AttackDirections { get; } =
    {
        (-1, 0),
        (1, 0),
        (0, -1),
        (0, 1)
    };
}