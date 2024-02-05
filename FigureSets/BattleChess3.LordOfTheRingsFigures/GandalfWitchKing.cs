using BattleChess3.DefaultFigures;

namespace BattleChess3.LordOfTheRingsFigures;

public class GandalfWitchKing : ILordOfTheRingsFigureType, IFigureTypeWithChainedMoves
{
    public static readonly GandalfWitchKing Instance = new();

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
