using BattleChess3.DefaultFigures;

namespace BattleChess3.SilmarillionFigures;

public class YavannaGlaurung : ISilmarillionFigureType, IFigureTypeWithChainedMoves
{
    public static readonly YavannaGlaurung Instance = new();
    
    int[] IFigureTypeWithChainedMoves.Actions { get; } = {
        3, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 3,
        0, 3, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 3, 0,
        0, 0, 3, 0, 0, 0, 0, 3, 0, 0, 0, 0, 3, 0, 0,
        0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0,
        0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 3, 0, 3, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0,
        3, 3, 3, 3, 3, 3, 3, 8, 3, 3, 3, 3, 3, 3, 3,
        0, 0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 3, 0, 3, 0, 3, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 3, 0, 0, 3, 0, 0, 3, 0, 0, 0, 0,
        0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0,
        0, 0, 3, 0, 0, 0, 0, 3, 0, 0, 0, 0, 3, 0, 0,
        0, 3, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 3, 0,
        3, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 3,
    };
}
