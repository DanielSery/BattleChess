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
    private static readonly Dictionary<Figure, int[]> Analysis = new Dictionary<Figure, int[]>();
    
    static FigureImpactAnalyzer()
    {
        var group = new CrownsGuardFigureTypeInfoGroup();
        foreach (IFigureTypeInfo groupFigureType in group.FigureTypes)
        {
            var neutralFigure = new Figure(PlayerColor.Neutral, false, groupFigureType.FigureId);
            Analysis[neutralFigure] = new int[Constants.FullBoardTilesCount]; 
            
            var whiteFigure = new Figure(PlayerColor.White, false, groupFigureType.FigureId);
            Analysis[whiteFigure] = AnalyzeFigureImpact(whiteFigure);
            
            var whiteKingFigure = new Figure(PlayerColor.White, true, groupFigureType.FigureId);
            Analysis[whiteKingFigure] = AnalyzeFigureImpact(whiteKingFigure);
            
            var blackFigure = new Figure(PlayerColor.Black, false, groupFigureType.FigureId);
            Analysis[blackFigure] = AnalyzeFigureImpact(blackFigure);
            
            var blackKingFigure = new Figure(PlayerColor.Black, true, groupFigureType.FigureId);
            Analysis[blackKingFigure] = AnalyzeFigureImpact(blackKingFigure);
        }
    }
    
    public static int GetFigureImpact(Figure figure, int index)
    {
        return Analysis[figure][index];
    }

    private static int[] AnalyzeFigureImpact(Figure figure)
    {
        var board = new Figure[Constants.FullBoardTilesCount];
        for (var j = 0; j < board.Length; j++)
        {
            board[j] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }
        
        var result = new int[Constants.FullBoardTilesCount];
        for (var i = 0; i < result.Length; i++)
        {
            using var clonedBoard = board.AsSpan().CloneToArrayPoolMemory();
            clonedBoard.Span[i] = figure;
            using var possibleActions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), clonedBoard.Span);
            
            var impact = 0;
            foreach (var possibleAction in possibleActions.Span)
            {
                impact += ActionImpactEvaluator.EvaluateAction(board, possibleAction, clonedBoard.Span[i]);
            }

            if (figure.PlayerColor == PlayerColor.White)
            {
                if (figure.IsKing) impact -= Constants.KingValue;
                impact -= figure.FigureType.GetFigureValue() * Constants.FigureValueCoeff;
            }
            else if (figure.PlayerColor == PlayerColor.Black)
            {
                if (figure.IsKing) impact += Constants.KingValue;
                impact += figure.FigureType.GetFigureValue() * Constants.FigureValueCoeff;
            }
            
            result[i] = impact;
        }
        
        return result;
    }
}