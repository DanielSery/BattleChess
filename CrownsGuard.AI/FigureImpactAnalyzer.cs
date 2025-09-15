// Copyright (c) Veeam Software Group GmbH

using System.Collections.Frozen;
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FigureDefinitions;
using CrownsGuard.FigureDefinitions.Utilities;

namespace CrownsGuard.AI;

public static class FigureImpactAnalyzer
{
    private static readonly CrownsGuardFigureTypeInfoGroup FigureTypes;

    static FigureImpactAnalyzer()
    {
        FigureTypes = new CrownsGuardFigureTypeInfoGroup();
    }

    public static FrozenDictionary<int, int[]> AnalyzeFigures(Span<Figure> board)
    {
        using var rentedTileImportance = BoardPool<int>.Rent(out var tileImportance);
        foreach (var figure in board)
        {
            var whiteFigure = figure.GetFigureType() | Figure.IsWhite;
            AnalyzeTileImportance(whiteFigure, tileImportance);

            var blackFigure = figure.GetFigureType() | Figure.IsBlack;
            AnalyzeTileImportance(blackFigure, tileImportance);
        }

        var maxImportance = tileImportance.Max();
        for (var i = 0; i < tileImportance.Length; i++)
        {
            tileImportance[i] = tileImportance[i] * 100 / maxImportance;
        }

        var analysis = new Dictionary<int, int[]>();
        foreach (var figureType in FigureTypes.FigureTypes)
        {
            var neutralFigure = figureType.Figure;
            analysis[(int)neutralFigure] = new int[Constants.FullBoardTilesCount];
            
            var whiteFigure = figureType.Figure | Figure.IsWhite;
            analysis[(int)whiteFigure] = AnalyzeFigureImpact(whiteFigure, tileImportance);
            
            var whiteKingFigure = figureType.Figure | Figure.IsWhite | Figure.IsKing;
            analysis[(int)whiteKingFigure] = AnalyzeFigureImpact(whiteKingFigure, tileImportance);
            
            var blackFigure = figureType.Figure | Figure.IsBlack;
            analysis[(int)blackFigure] = AnalyzeFigureImpact(blackFigure, tileImportance);

            var blackKingFigure = figureType.Figure | Figure.IsBlack | Figure.IsKing;
            analysis[(int)blackKingFigure] = AnalyzeFigureImpact(blackKingFigure, tileImportance);
        }
        
        return analysis.ToFrozenDictionary();
    }

    private static void AnalyzeTileImportance(Figure figure, int[] tileImportance)
    {
        using var rentedBoard = BoardPool<Figure>.Rent(out var board);
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = Figure.Empty;
        }

        using var rentedClonedBoard = BoardPool<Figure>.Rent(out var clonedBoard);
        using var rentedActionsStack = ActionStackPool.Rent(out var actionsStack);
        for (var i = 0; i < tileImportance.Length; i++)
        {
            board.CopyTo(clonedBoard, 0);
            clonedBoard[i] = figure;

            var impact = 0;
            var countBefore = actionsStack.Count;
            FigureActionsResolver.GetPossibleActions(i, figure, clonedBoard, actionsStack);
            var addedElements = actionsStack.Count - countBefore;

            for (var j = 0; j < addedElements; j++)
            {
                var possibleAction = actionsStack.Pop();
                var value = Math.Abs(ActionImpactEvaluator.EvaluateAction(board, possibleAction, figure.GetFigureColor()));
                impact += value;
            }

            tileImportance[i] += impact;
        }
    }

    private static int[] AnalyzeFigureImpact(Figure figure, int[] tileImportance)
    {
        using var rentedBoard = BoardPool<Figure>.Rent(out var board);
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = Figure.Empty;
        }
        
        using var rentedClonedBoard = BoardPool<Figure>.Rent(out var clonedBoard);
        using var rentedActionsStack = ActionStackPool.Rent(out var actionsStack);
        _ = BoardPool<int>.Rent(out var result);
        for (var i = 0; i < result.Length; i++)
        {
            board.CopyTo(clonedBoard, 0);
            clonedBoard[i] = figure;

            var impact = 0;
            var countBefore = actionsStack.Count;
            FigureActionsResolver.GetPossibleActions(i, figure, clonedBoard, actionsStack);
            var addedElements = actionsStack.Count - countBefore;

            for (var j = 0; j < addedElements; j++)
            {
                var possibleAction = actionsStack.Pop();
                impact += (ActionImpactEvaluator.EvaluateAction(board, possibleAction, figure.GetFigureColor()) * tileImportance[i]) / 2000;
            }

            if (figure.IsKing())
            {
                result[i] = figure.GetFigureColor() switch
                {
                    Figure.IsBlack => Constants.KingValue - tileImportance[i],
                    Figure.IsWhite => -(Constants.KingValue - tileImportance[i]),
                    _ => result[i]
                };
                continue;
            }

            impact += figure.GetFigureValue() * Constants.FigureValueCoeff;
            result[i] = figure.GetFigureColor() switch
            {
                Figure.IsBlack => impact,
                Figure.IsWhite => -impact,
                _ => result[i]
            };
        }
        
        return result;
    }
}