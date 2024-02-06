using BattleChess3.DefaultFigures;

namespace BattleChess3.CrossFireFigures;

public class Archer : ICrossFireFigureType, IFigureTypeWithChainedRangeAttack
{
    public static readonly Archer Instance = new();

    int[] IFigureTypeWithChainedRangeAttack.Actions { get; } =
    {
        2, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2,
        0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0,
        0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0,
        0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0,
        0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 0, 2, 0, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 1, 8, 1, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 2, 2, 2, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 2, 0, 2, 0, 2, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0,
        0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0,
        0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0,
        0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0,
        2, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 2
    };
}