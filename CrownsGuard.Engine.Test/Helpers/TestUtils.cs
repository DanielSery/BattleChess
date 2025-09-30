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
        return (e, _) => sink.Add(e);
    }

    public static object RunGetActionsScenario(
        string name,
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

        return new { scenario = name, figure = figureName, actions = result };
    }

    public static object RunExecuteActionScenario(
        string name,
        Action<Span<Figure>> setup,
        int src,
        Figure figure,
        short relative,
        FigureActionType actionType)
    {
        var sourceBoard = EmptyBoard();
        sourceBoard[src] = figure;
        setup(sourceBoard);

        var dst = src.GetWithOffset(relative);
        if (dst == -1) throw new ArgumentOutOfRangeException(nameof(relative), "Destination is out of board");
        var resultBoard = sourceBoard.ToArray();

        var action = new FigureAction(actionType, src, dst, figure);
        FigureActionExecutor.ExecuteFigureAction(resultBoard, action, IgnoreEvents);

        var nonEmpty = Enumerable.Range(0, 64)
            .Where(i => resultBoard[i] != sourceBoard[i])
            .Select(i => $"({i/8},{i%8}):({sourceBoard[i]})->({resultBoard[i]})")
            .ToArray();
        
        var actionString = $"({src / 8},{src % 8}->{dst/8},{dst%8}):{actionType}";
        return new { scenario = name, action = actionString, board = nonEmpty};
    }

    public static object RunExecuteActionScenarioWithEventHandling(
        string name,
        Action<Span<Figure>> setup,
        int src,
        Figure figure,
        short relative,
        FigureActionType actionType)
    {
        var sourceBoard = EmptyBoard();
        sourceBoard[src] = figure;
        setup(sourceBoard);

        var dst = src.GetWithOffset(relative);
        if (dst == -1) throw new ArgumentOutOfRangeException(nameof(relative), "Destination is out of board");
        var resultBoard = sourceBoard.ToArray();

        var action = new FigureAction(actionType, src, dst, figure);
        FigureActionExecutor.ExecuteFigureAction(resultBoard, action, BoardEventHandler.HandleFigureActionEvent);

        var nonEmpty = Enumerable.Range(0, 64)
            .Where(i => resultBoard[i] != sourceBoard[i])
            .Select(i => $"({i/8},{i%8}):({sourceBoard[i]})->({resultBoard[i]})")
            .ToArray();
        
        var actionString = $"({src / 8},{src % 8}->{dst/8},{dst%8}):{actionType}";
        return new { scenario = name, action = actionString, board = nonEmpty};
    }

    public static Action<BoardEvent, Span<Figure>> IgnoreEvents => (boardEvent, board) => { };
}