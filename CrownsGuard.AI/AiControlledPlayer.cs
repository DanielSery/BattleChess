using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.AI;

public class AiControlledPlayer : IAutomaticallyControlledPlayerInfo
{
    private readonly Figure[] _board;
    private readonly Action<Position, Position, TimeSpan> _requestMove;
    private readonly int _difficulty;
    private readonly Random _random;

    public AiControlledPlayer(PlayerColor playerColor, Figure[] board, Action<Position, Position, TimeSpan> requestMove, int difficulty)
    {
        _random = new Random();
        _board = board;
        _requestMove = requestMove;
        _difficulty = difficulty;
        PlayerColor = playerColor;
    }

    /// <inheritdoc />
    public string Name => "AI Controlled Player";

    /// <inheritdoc />
    public PlayerColor PlayerColor { get; }

    /// <inheritdoc />
    public IPlayerTimer Timer { get; } = InfinitePlayerTimer.Instance;

    /// <inheritdoc />
    public void SetTimer(IPlayerTimer timer)
    {
    }

    /// <inheritdoc />
    public void StartTurn()
    {
    }

    /// <inheritdoc />
    public void EndTurn(TimeSpan? forcedTurnDuration = null)
    {
    }

    /// <inheritdoc />
    public Task HandleAutomaticTurnAsync()
    {
        return Task.Run(() =>
        {
            if (PlayerColor == PlayerColor.White)
                HandleWhitePlayer();
            else HandleBlackPlayer();
        });
    }

    private void HandleBlackPlayer()
    {
        var sw = new Stopwatch();
        sw.Start();

        var analysis = FigureImpactAnalyzer.AnalyzeFigures(_board);
        var resultActions = new ConcurrentBag<(int value, FigureAction action)>();

        var checkedActions = new Stack<FigureAction>();
        for (var i = 0; i < _board.Length; i++)
        {
            var index = i;
            var figure = _board[i];
            if (figure.PlayerColor != PlayerColor.Black)
                continue;

            var actionsStack = new Stack<FigureAction>(64);
            FigureActionsResolver.GetPossibleActions(Position.FromIndex(index), _board, actionsStack);
            foreach (var action in actionsStack)
            {
                if (!action.FigureActionType.IsExecutable())
                    continue;

                checkedActions.Push(action);
            }
        }

        Parallel.ForEach(checkedActions, action =>
        {
            using var clonedBoard = _board.AsSpan().CloneToArrayPoolMemory();
            FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
            var eval = AlphaBeta(clonedBoard.Span, _difficulty - 1, int.MinValue, int.MaxValue, false, analysis, new Stack<FigureAction>(64));
            resultActions.Add((eval, action));
            // Console.WriteLine($"Evaluated action {sw.Elapsed} action: {action}, eval: {eval}");
        });

        var resultArray = resultActions.ToList();
        var alpha = resultArray.Max(x => x.value);
        for (var i = resultActions.Count - 1; i >= 0; i--)
        {
            if (resultArray[i].value < alpha - 900)
                resultArray.RemoveAt(i);
        }

        var executedAction = resultArray[_random.Next(0, resultArray.Count)];
        Console.WriteLine($"Time for turn: {sw.Elapsed}, evaluation: {executedAction.value}");
        _requestMove.Invoke(executedAction.action.SourcePosition, executedAction.action.TargetPosition, TimeSpan.Zero);
    }

    private void HandleWhitePlayer()
    {
        var sw = new Stopwatch();
        sw.Start();

        var analysis = FigureImpactAnalyzer.AnalyzeFigures(_board);
        var resultActions = new ConcurrentBag<(int value, FigureAction action)>();

        var checkedActions = new Stack<FigureAction>();
        for (var i = 0; i < _board.Length; i++)
        {
            var index = i;
            var figure = _board[i];
            if (figure.PlayerColor != PlayerColor.White)
                continue;

            var actionsStack = new Stack<FigureAction>(64);
            FigureActionsResolver.GetPossibleActions(Position.FromIndex(index), _board, actionsStack);
            foreach (var action in actionsStack)
            {
                if (!action.FigureActionType.IsExecutable())
                    continue;

                checkedActions.Push(action);
            }
        }

        Parallel.ForEach(checkedActions, action =>
        {
            using var clonedBoard = _board.AsSpan().CloneToArrayPoolMemory();
            FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
            var eval = AlphaBeta(clonedBoard.Span, _difficulty - 1, int.MinValue, int.MaxValue, true, analysis, new Stack<FigureAction>(64));
            resultActions.Add((eval, action));
            // Console.WriteLine($"Evaluated action {sw.Elapsed} action: {action}, eval: {eval}");
        });

        var resultArray = resultActions.ToList();
        var beta = resultArray.Min(x => x.value);
        for (var i = resultActions.Count - 1; i >= 0; i--)
        {
            if (resultArray[i].value > beta + 900)
                resultArray.RemoveAt(i);
        }

        var executedAction = resultArray[_random.Next(0, resultArray.Count)];
        Console.WriteLine($"Time for turn: {sw.Elapsed}, evaluation: {executedAction.value}");
        _requestMove.Invoke(executedAction.action.SourcePosition, executedAction.action.TargetPosition, TimeSpan.Zero);
    }

    private int AlphaBeta(ReadOnlySpan<Figure> board, int depth, int alpha, int beta, bool maximizingPlayer, FrozenDictionary<int, int[]> analysis, Stack<FigureAction> actionsStack)
    {
        var check = EvaluateBoard(board, analysis, actionsStack);
        if (depth == 0 || Math.Abs(check) > 10_000_000)
        {
            return check;
        }

        if (maximizingPlayer)
        {
            var maxEval = int.MinValue;
            for (var i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.PlayerColor != PlayerColor.Black)
                    continue;

                var oldCount = actionsStack.Count;
                FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board, actionsStack);
                var added = actionsStack.Count - oldCount;
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    if (beta <= alpha)
                        continue; // Beta cut-off

                    if (!action.FigureActionType.IsExecutable())
                        continue;

                    using var clonedBoard = board.CloneToArrayPoolMemory();
                    FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
                    var eval = AlphaBeta(clonedBoard.Span, depth - 1, alpha, beta, false, analysis, actionsStack);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                }
            }
            return maxEval;
        }
        else
        {
            var minEval = int.MaxValue;
            for (var i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.PlayerColor != PlayerColor.White)
                    continue;

                var oldCount = actionsStack.Count;
                FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board, actionsStack);
                var added = actionsStack.Count - oldCount;
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    if (beta <= alpha)
                        continue; // Alpha cut-off

                    if (!action.FigureActionType.IsExecutable())
                        continue;

                    using var clonedBoard = board.CloneToArrayPoolMemory();
                    FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
                    var eval = AlphaBeta(clonedBoard.Span, depth - 1, alpha, beta, true, analysis, actionsStack);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                }
            }
            
            return minEval;
        }
    }

    private void OnEvent(BoardEvent boardEvent, Span<Figure> board)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static int EvaluateBoard(ReadOnlySpan<Figure> board, FrozenDictionary<int, int[]> analysis, Stack<FigureAction> actionsStack)
    {
        var evaluation = 0;
        for (var i = 0; i < board.Length; i++)
        {
            var figure = board[i];
            evaluation += analysis[figure.IntValue][i];
        }

        if (Math.Abs(evaluation) > 100_000_000)
        {
            return evaluation;
        }

        for (var i = 0; i < board.Length; i++)
        {
            var figure = board[i];
            if (figure.PlayerColor == PlayerColor.Neutral)
            {
                continue;
            }

            var oldCount = actionsStack.Count;
            FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board, actionsStack);
            var added = actionsStack.Count - oldCount;
            if (figure.PlayerColor == PlayerColor.White)
            {
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    evaluation -= ActionImpactEvaluator.EvaluateAction(board, action, figure);
                }
            }
            else
            {
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    evaluation += ActionImpactEvaluator.EvaluateAction(board, action, figure);
                }
            }
        }
        return evaluation;
    }
}