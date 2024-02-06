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

public interface IDoubleChessFigureType : IFigureType
{
    string IFigureType.UnitName => $"{nameof(DoubleChessFigureGroup)}.{GetType().Name}";
    string IFigureType.DisplayName => CurrentLocalization.Instance[$"{GetType().Name}_Name"];
    string IFigureType.Description => CurrentLocalization.Instance[$"{GetType().Name}_{nameof(Description)}"];

    IDictionary<int, Uri> IFigureType.ImageUris => new Dictionary<int, Uri>
    {
        {
            1,
            new Uri($"pack://application:,,,/BattleChess3.DoubleChessFigures;component/Images/{GetType().Name}1.png",
                UriKind.Absolute)
        },
        {
            2,
            new Uri($"pack://application:,,,/BattleChess3.DoubleChessFigures;component/Images/{GetType().Name}2.png",
                UriKind.Absolute)
        }
    };

    FigureAction CreateMergeAction(ITile tile1, ITile tile2)
    {
        var figure1 = tile1.Figure.FigureType;
        var figure2 = tile2.Figure.FigureType;
        switch (figure1)
        {
            case SinglePawn when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, PawnPawn.Instance);
            case SinglePawn when figure2 is SingleBishop:
            case SingleBishop when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, BishopPawn.Instance);
            case SinglePawn when figure2 is SingleKnight:
            case SingleKnight when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, KnightPawn.Instance);
            case SinglePawn when figure2 is SingleRook:
            case SingleRook when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, RookPawn.Instance);
            case SinglePawn when figure2 is SingleQueen:
            case SingleQueen when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, QueenPawn.Instance);
            case SinglePawn when figure2 is SingleKing:
            case SingleKing when figure2 is SinglePawn:
                return CreateMergeAction(tile1, tile2, KingPawn.Instance);
            case SingleBishop when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, BishopBishop.Instance);
            case SingleBishop when figure2 is SingleKnight:
            case SingleKnight when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, KnightBishop.Instance);
            case SingleBishop when figure2 is SingleRook:
            case SingleRook when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, RookBishop.Instance);
            case SingleBishop when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, QueenBishop.Instance);
            case SingleBishop when figure2 is SingleKing:
            case SingleKing when figure2 is SingleBishop:
                return CreateMergeAction(tile1, tile2, KingBishop.Instance);
            case SingleKnight when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, KnightKnight.Instance);
            case SingleKnight when figure2 is SingleRook:
            case SingleRook when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, RookKnight.Instance);
            case SingleKnight when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, QueenKnight.Instance);
            case SingleKnight when figure2 is SingleKing:
            case SingleKing when figure2 is SingleKnight:
                return CreateMergeAction(tile1, tile2, KingKnight.Instance);
            case SingleRook when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, RookRook.Instance);
            case SingleRook when figure2 is SingleQueen:
            case SingleQueen when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, QueenRook.Instance);
            case SingleRook when figure2 is SingleKing:
            case SingleKing when figure2 is SingleRook:
                return CreateMergeAction(tile1, tile2, KingRook.Instance);
            case SingleQueen when figure2 is SingleQueen:
                return CreateMergeAction(tile1, tile2, QueenQueen.Instance);
            case SingleQueen when figure2 is SingleKing:
            case SingleKing when figure2 is SingleQueen:
                return CreateMergeAction(tile1, tile2, KingQueen.Instance);
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

    FigureAction CreatedCombinedActions(
        ITile tile1,
        ITile tile2,
        ITile[] board,
        IFigureType figure1,
        IFigureType figure2)
    {
        var firstUnitAction = figure1.GetPossibleAction(tile1, tile2, board);
        if (firstUnitAction.ActionType != FigureActionTypes.None)
            return new FigureAction(firstUnitAction.ActionType, () =>
            {
                var owner = tile1.Figure.Owner;
                tile1.Die();
                tile1.CreateFigure(new Figure(owner, figure1));
                firstUnitAction.Action.Invoke();
                tile1.CreateFigure(new Figure(owner, figure2));
            });
        
        var secondUnitAction = figure2.GetPossibleAction(tile1, tile2, board);
        if (secondUnitAction.ActionType != FigureActionTypes.None)
            return new FigureAction(secondUnitAction.ActionType, () =>
            {
                var owner = tile1.Figure.Owner;
                tile1.Die();
                tile1.CreateFigure(new Figure(owner, figure2));
                secondUnitAction.Action.Invoke();
                tile1.CreateFigure(new Figure(owner, figure1));
            });

        return new FigureAction(FigureActionTypes.None, () => { });
    }
}