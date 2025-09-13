// Copyright (c) Veeam Software Group GmbH

using System.Collections.Frozen;
using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
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
            var whiteFigure = new Figure(PlayerColor.White, false, figure.FigureType);
            AnalyzeTileImportance(whiteFigure, tileImportance);

            var blackFigure = new Figure(PlayerColor.Black, false, figure.FigureType);
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
            var neutralFigure = new Figure(PlayerColor.Neutral, false, figureType.FigureId);
            analysis[neutralFigure.IntValue] = new int[Constants.FullBoardTilesCount];
            
            var whiteFigure = new Figure(PlayerColor.White, false, figureType.FigureId);
            analysis[whiteFigure.IntValue] = AnalyzeFigureImpact(whiteFigure, tileImportance);
            
            var whiteKingFigure = new Figure(PlayerColor.White, true, figureType.FigureId);
            analysis[whiteKingFigure.IntValue] = AnalyzeFigureImpact(whiteKingFigure, tileImportance);
            
            var blackFigure = new Figure(PlayerColor.Black, false, figureType.FigureId);
            analysis[blackFigure.IntValue] = AnalyzeFigureImpact(blackFigure, tileImportance);

            var blackKingFigure = new Figure(PlayerColor.Black, true, figureType.FigureId);
            analysis[blackKingFigure.IntValue] = AnalyzeFigureImpact(blackKingFigure, tileImportance);
        }
        
        return analysis.ToFrozenDictionary();
    }

    private static void AnalyzeTileImportance(Figure figure, int[] tileImportance)
    {
        using var rentedBoard = BoardPool<Figure>.Rent(out var board);
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }

        using var rentedClonedBoard = BoardPool<Figure>.Rent(out var clonedBoard);
        using var rentedActionsStack = ActionStackPool.Rent(out var actionsStack);
        for (var i = 0; i < tileImportance.Length; i++)
        {
            board.CopyTo(clonedBoard, 0);
            clonedBoard[i] = figure;

            var impact = 0;
            var countBefore = actionsStack.Count;
            FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), clonedBoard, actionsStack);
            var addedElements = actionsStack.Count - countBefore;

            for (var j = 0; j < addedElements; j++)
            {
                var possibleAction = actionsStack.Pop();
                var value = Math.Abs(ActionImpactEvaluator.EvaluateAction(board, possibleAction));
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
            board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
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
            FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), clonedBoard, actionsStack);
            var addedElements = actionsStack.Count - countBefore;

            for (var j = 0; j < addedElements; j++)
            {
                var possibleAction = actionsStack.Pop();
                impact += (ActionImpactEvaluator.EvaluateAction(board, possibleAction) * tileImportance[i]) / 1000;
            }

            if (figure.IsKing)
            {
                result[i] = figure.PlayerColor switch
                {
                    PlayerColor.Black => Constants.KingValue - tileImportance[i],
                    PlayerColor.White => -(Constants.KingValue - tileImportance[i]),
                    _ => result[i]
                };
                continue;
            }

            impact += figure.FigureType.GetFigureValue() * (Constants.FigureValueCoeff - tileImportance[i]);
            result[i] = figure.PlayerColor switch
            {
                PlayerColor.Black => impact,
                PlayerColor.White => -impact,
                _ => result[i]
            };
        }
        
        return result;
    }
}