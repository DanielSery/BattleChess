using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using BattleChess3.DefaultFigures.Utilities;
using BattleChess3.DoubleChessFigures.Localization;

namespace BattleChess3.DoubleChessFigures;

internal interface IDoubleChessFigureType : IFigureType
{
    string IFigureType.UnitName => $"{nameof(DoubleChessFigureGroup)}.{GetType().Name}";
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_Name"];
    string IFigureType.Description => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(Description)}"];

    IDictionary<int, Uri> IFigureType.ImageUris =>
        new Dictionary<int, Uri>
        {
            { 1, new Uri($"pack://application:,,,/BattleChess3.DoubleChessFigures;component/Images/{GetType().Name}1.png", UriKind.Absolute) },
            { 2, new Uri($"pack://application:,,,/BattleChess3.DoubleChessFigures;component/Images/{GetType().Name}2.png", UriKind.Absolute) }
        };

    FigureAction CreateMergeAction(ITile tile1, ITile tile2, IBoard board)
    {
        var figure1 = this;
        var figure2 = tile2.Figure.FigureType;
        switch (figure1)
        {
            case SinglePawn when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.PawnPawn, board);
            case SinglePawn when figure2 is SingleBishop:
            case SingleBishop when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.BishopPawn, board);
            case SinglePawn when figure2 is SingleKnight:
            case SingleKnight when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KnightPawn, board);
            case SinglePawn when figure2 is SingleRook:
            case SingleRook when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookPawn, board);
            case SinglePawn when figure2 is SingleQueen:
            case SingleQueen when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenPawn, board);
            case SinglePawn when figure2 is SingleKing:
            case SingleKing when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingPawn, board);
            case SingleBishop when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.BishopBishop, board);
            case SingleBishop when figure2 is SingleKnight:
            case SingleKnight when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KnightBishop, board);
            case SingleBishop when figure2 is SingleRook:
            case SingleRook when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookBishop, board);
            case SingleBishop when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenBishop, board);
            case SingleBishop when figure2 is SingleKing:
            case SingleKing when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingBishop, board);
            case SingleKnight when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KnightKnight, board);
            case SingleKnight when figure2 is SingleRook:
            case SingleRook when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookKnight, board);
            case SingleKnight when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenKnight, board);
            case SingleKnight when figure2 is SingleKing:
            case SingleKing when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingKnight, board);
            case SingleRook when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookRook, board);
            case SingleRook when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenRook, board);
            case SingleRook when figure2 is SingleKing:
            case SingleKing when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingRook, board);
            case SingleQueen when figure2 is SingleQueen:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenQueen, board);
            case SingleQueen when figure2 is SingleKing:
            case SingleKing when figure2 is SingleQueen:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingQueen, board);
            default:
                return new FigureAction(FigureActionTypes.None, () => { });
        }
    }

    FigureAction CreateMergeAction(ITile tile1, ITile tile2, IFigureType figureType, IBoard board)
    {
        return new FigureAction(FigureActionTypes.Special, () =>
        {
            tile2.Die(board);
            tile2.CreateFigure(new Figure(tile1.Figure.Owner, figureType), board);
            tile1.Die(board);
        });
    }
}