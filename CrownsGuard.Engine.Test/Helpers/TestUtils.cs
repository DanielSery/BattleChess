using System;
using System.Collections.Generic;
using System.Linq;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Engine.Figures;
using CrownsGuard.Engine.Helpers;

namespace CrownsGuard.Engine.Test.Helpers;

internal static class TestUtils
{
    public static Figure[] EmptyBoard()
    {
        return new Figure[64];
    }

    public static Action<BoardEvent, Span<Figure>> CaptureEvents(List<BoardEvent> sink)
    {
        return (e, b) => sink.Add(e);
    }

    public static object RunGetActionsScenario(
        Action<Span<Figure>> setup, 
        int src,
        Figure figure, 
        Action<int, Figure, ReadOnlySpan<Figure>, Stack<FigureAction>> getPossibleActions)
    {
        var board = EmptyBoard();
        board[src] = figure;
        setup(board);

        var actions = new Stack<FigureAction>();
        getPossibleActions.Invoke(src, figure, board, actions);
        var result = actions
            .OrderBy(a => a.TargetIndex)
            .Select(a => $"({a.TargetIndex/8},{a.TargetIndex%8}):{a.FigureActionType}")
            .ToArray();
        var figureName = $"({src / 8},{src % 8})";

        return new { figure = figureName, actions = result };
    }

    public static object RunExecuteActionScenario(
        Action<Span<Figure>> setup,
        int src,
        Figure figure,
        short relative,
        FigureActionType actionType)
    {
        var board = EmptyBoard();
        board[src] = figure;
        setup(board);

        var dst = src.GetWithOffset(relative);
        if (dst == -1) throw new ArgumentOutOfRangeException(nameof(relative), "Destination is out of board");

        var action = new FigureAction(actionType, src, dst, figure);
        FigureActionExecutor.ExecuteFigureAction(board, action, IgnoreEvents());

        var nonEmpty = Enumerable.Range(0, 64)
            .Where(i => board[i] != Figure.Empty)
            .Select(i => $"({i/8},{i%8}):{board[i]}")
            .ToArray();
        
        var actionString = $"({src / 8},{src % 8}->{dst/8},{dst%8}):{actionType}";
        return new { action = actionString, board = nonEmpty};
    }

    public static Action<BoardEvent, Span<Figure>> IgnoreEvents() => (_, _) => { };
}