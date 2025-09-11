// Copyright (c) Veeam Software Group GmbH

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

    public static Dictionary<Figure, int[]> AnalyzeFigures(Span<Figure> board)
    {
        var averageAnalysis = new int[Constants.FullBoardTilesCount];
        foreach (Figure figure in board)
        {
            var whiteFigure = new Figure(PlayerColor.White, false, figure.FigureType);
            AnalyzeAverageTile(whiteFigure, averageAnalysis);
            
            var blackFigure = new Figure(PlayerColor.Black, false, figure.FigureType);
            AnalyzeAverageTile(blackFigure, averageAnalysis);
        }
        
        for (var i = 0; i < averageAnalysis.Length; i++)
        {
            averageAnalysis[i] = averageAnalysis[i] / board.Length / 2;
        }
        
        var analysis = new Dictionary<Figure, int[]>();
        foreach (var figureType in FigureTypes.FigureTypes)
        {
            var neutralFigure = new Figure(PlayerColor.Neutral, false, figureType.FigureId);
            analysis[neutralFigure] = new int[Constants.FullBoardTilesCount]; 
            
            var whiteFigure = new Figure(PlayerColor.White, false, figureType.FigureId);
            analysis[whiteFigure] = AnalyzeFigureImpact(whiteFigure, averageAnalysis);
            
            var whiteKingFigure = new Figure(PlayerColor.White, true, figureType.FigureId);
            analysis[whiteKingFigure] = AnalyzeFigureImpact(whiteKingFigure, averageAnalysis);
            
            var blackFigure = new Figure(PlayerColor.Black, false, figureType.FigureId);
            analysis[blackFigure] = AnalyzeFigureImpact(blackFigure, averageAnalysis);
            
            var blackKingFigure = new Figure(PlayerColor.Black, true, figureType.FigureId);
            analysis[blackKingFigure] = AnalyzeFigureImpact(blackKingFigure, averageAnalysis);
        }
        
        return analysis;
    }

    private static int[] AnalyzeAverageTile(Figure figure, int[] array)
    {
        var board = new Figure[Constants.FullBoardTilesCount];
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }
        
        for (var i = 0; i < array.Length; i++)
        {
            using var clonedBoard = board.AsSpan().CloneToArrayPoolMemory();
            clonedBoard.Span[i] = figure;
            using var possibleActions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), clonedBoard.Span);
            
            var impact = 0;
            foreach (var possibleAction in possibleActions.Span)
            {
                impact += Math.Abs(ActionImpactEvaluator.EvaluateAction(board, possibleAction, clonedBoard.Span[i]));
            }
            
            array[i] += impact;
        }
        
        return array;
    }

    private static int[] AnalyzeFigureImpact(Figure figure, int[] averageValue)
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
                impact += ActionImpactEvaluator.EvaluateAction(board, possibleAction, clonedBoard.Span[i]) * averageValue[i];
            }

            if (figure.IsKing) impact += Constants.KingValue;
            impact += figure.FigureType.GetFigureValue() * Constants.FigureValueCoeff;

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