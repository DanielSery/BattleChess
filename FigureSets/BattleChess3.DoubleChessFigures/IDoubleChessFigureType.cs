#region Copyright FEI Company 2024

// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.

#endregion

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

    FigureAction CreateMergeAction(ITile tile1, ITile tile2)
    {
        var figure1 = this;
        var figure2 = tile2.Figure.FigureType;
        switch (figure1)
        {
            case SinglePawn when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.PawnPawn);
            case SinglePawn when figure2 is SingleBishop:
            case SingleBishop when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.BishopPawn);
            case SinglePawn when figure2 is SingleKnight:
            case SingleKnight when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KnightPawn);
            case SinglePawn when figure2 is SingleRook:
            case SingleRook when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookPawn);
            case SinglePawn when figure2 is SingleQueen:
            case SingleQueen when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenPawn);
            case SinglePawn when figure2 is SingleKing:
            case SingleKing when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingPawn);
            case SingleBishop when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.BishopBishop);
            case SingleBishop when figure2 is SingleKnight:
            case SingleKnight when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KnightBishop);
            case SingleBishop when figure2 is SingleRook:
            case SingleRook when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookBishop);
            case SingleBishop when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenBishop);
            case SingleBishop when figure2 is SingleKing:
            case SingleKing when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingBishop);
            case SingleKnight when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KnightKnight);
            case SingleKnight when figure2 is SingleRook:
            case SingleRook when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookKnight);
            case SingleKnight when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenKnight);
            case SingleKnight when figure2 is SingleKing:
            case SingleKing when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingKnight);
            case SingleRook when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.RookRook);
            case SingleRook when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenRook);
            case SingleRook when figure2 is SingleKing:
            case SingleKing when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingRook);
            case SingleQueen when figure2 is SingleQueen:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.QueenQueen);
            case SingleQueen when figure2 is SingleKing:
            case SingleKing when figure2 is SingleQueen:
                return CreateMergeAction(tile1, tile2, DoubleChessFigureGroup.KingQueen);
            default:
                return new FigureAction(FigureActionTypes.None, () => { });
        }
    }

    FigureAction CreateMergeAction(ITile tile1, ITile tile2, IFigureType figureType)
    {
        return new FigureAction(FigureActionTypes.Special, () =>
        {
            tile2.Die();
            tile2.CreateFigure(new Figure(tile1.Figure.Owner, figureType));
            tile1.Die();
        });
    }
}