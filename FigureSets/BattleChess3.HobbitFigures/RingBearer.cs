using BattleChess3.DefaultFigures;

namespace BattleChess3.HobbitFigures;

public class RingBearer : IHobbitFigureType, IFigureTypeWithChainedMoves
{
    public static readonly RingBearer Instance = new();
    
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
