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
        var tileImportance = new int[Constants.FullBoardTilesCount];
        foreach (Figure figure in board)
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
        var board = new Figure[Constants.FullBoardTilesCount];
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }
        
        for (var i = 0; i < tileImportance.Length; i++)
        {
            using var clonedBoard = board.AsSpan().CloneToArrayPoolMemory();
            clonedBoard.Span[i] = figure;
            using var possibleActions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), clonedBoard.Span);
            
            var impact = 0;
            foreach (var possibleAction in possibleActions.Span)
            {
                var value = Math.Abs(ActionImpactEvaluator.EvaluateAction(board, possibleAction, clonedBoard.Span[i]));
                impact += value;
            }
            
            tileImportance[i] += impact;
        }
    }

    private static int[] AnalyzeFigureImpact(Figure figure, int[] tileImportance)
    {
        var board = new Figure[Constants.FullBoardTilesCount];
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }
        
        var array = new int[Constants.FullBoardTilesCount];
        for (var i = 0; i < array.Length; i++)
        {
            using var clonedBoard = board.AsSpan().CloneToArrayPoolMemory();
            clonedBoard.Span[i] = figure;
            using var possibleActions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), clonedBoard.Span);
            
            var impact = 0;
            foreach (var possibleAction in possibleActions.Span)
            {
                impact += (ActionImpactEvaluator.EvaluateAction(board, possibleAction, clonedBoard.Span[i]) * tileImportance[i]) / 1000;
            }

            if (figure.IsKing)
            {
                array[i] = figure.PlayerColor switch
                {
                    PlayerColor.Black => Constants.KingValue - tileImportance[i],
                    PlayerColor.White => -(Constants.KingValue - tileImportance[i]),
                    _ => array[i]
                };
                continue;
            }

            impact += figure.FigureType.GetFigureValue() * (Constants.FigureValueCoeff - tileImportance[i]);
            array[i] = figure.PlayerColor switch
            {
                PlayerColor.Black => impact,
                PlayerColor.White => -impact,
                _ => array[i]
            };
        }
        
        return array;
    }
}