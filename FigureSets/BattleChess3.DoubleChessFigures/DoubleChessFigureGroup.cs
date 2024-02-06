using BattleChess3.Core.Model.Figures;
using BattleChess3.DoubleChessFigures.Localization;

namespace BattleChess3.DoubleChessFigures;

public class DoubleChessFigureGroup : IFigureGroup
{
    public string DisplayName => CurrentLocalization.Instance[$"{nameof(DoubleChessFigureGroup)}_Name"];

    public IFigureType[] FigureTypes => new IFigureType[]
    {
        SingleKing.Instance,
        KingQueen.Instance, 
        SingleQueen.Instance,
        QueenQueen.Instance, 
        QueenKnight.Instance, 
        QueenBishop.Instance, 
        QueenPawn.Instance, 
        SingleRook.Instance,
        RookRook.Instance, 
        RookKnight.Instance, 
        RookBishop.Instance, 
        RookPawn.Instance, 
        SingleKnight.Instance,
        KnightKnight.Instance, 
        KnightBishop.Instance, 
        KnightPawn.Instance, 
        SingleBishop.Instance,
        BishopBishop.Instance, 
        BishopPawn.Instance, 
        SinglePawn.Instance,
        PawnPawn.Instance,
    };
}