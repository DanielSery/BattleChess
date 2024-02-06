using BattleChess3.DefaultFigures;

namespace BattleChess3.LordOfTheRingsFigures;

public class LegolasNazgul : ILordOfTheRingsFigureType, IFigureTypeWithChainedMoves
{
    public static readonly LegolasNazgul Instance = new();

    int[] IFigureTypeWithChainedMoves.Actions { get; } =
    {
        1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1,
        0, 1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 1, 0,
        0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1, 0, 0,
        0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 1, 0, 0, 0,
        0, 0, 0, 0, 1, 0, 0, 2, 0, 0, 1, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 2, 0, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0, 0, 0,
        2, 2, 2, 2, 2, 2, 2, 8, 2, 2, 2, 2, 2, 2, 2,
        0, 0, 0, 0, 0, 0, 1, 2, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 2, 0, 1, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 1, 0, 0, 2, 0, 0, 1, 0, 0, 0, 0,
        0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 1, 0, 0, 0,
        0, 0, 1, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1, 0, 0,
        0, 1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 1, 0,
        1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1
    };
}