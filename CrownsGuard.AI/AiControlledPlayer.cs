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

public sealed class AiControlledPlayer : IAutomaticallyControlledPlayerInfo
{
    private readonly Figure[] _board;
    private readonly Action<byte, byte, TimeSpan> _requestMove;
    private readonly int _difficulty;
    private readonly Random _random;
    private FrozenDictionary<int, int[]> _analysis;

    public AiControlledPlayer(PlayerColor playerColor, Figure[] board, Action<byte, byte, TimeSpan> requestMove, int difficulty)
    {
        _random = new Random();
        _board = board;
        _requestMove = requestMove;
        _difficulty = difficulty;
        _analysis = FrozenDictionary<int, int[]>.Empty;
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

        _analysis = FigureImpactAnalyzer.AnalyzeFigures(_board);
        try
        {
            var resultActions = new ConcurrentBag<(int value, FigureAction action)>();
            using var rentedCheckedActions = ActionStackPool.Rent(out var checkedActions);
            using var rentedCurrentActions = ActionStackPool.Rent(out var currentActions);

            for (var i = 0; i < _board.Length; i++)
            {
                var figure = _board[i];
                if (figure.IsWhite())
                    continue;

                var countBefore = currentActions.Count;
                FigureActionsResolver.GetPossibleActions(i, figure, _board, currentActions);
                var added = currentActions.Count - countBefore;

                for (var j = 0; j < added; j++)
                {
                    var action = currentActions.Pop();
                    if (!action.FigureActionType.IsExecutable())
                        continue;

                    checkedActions.Push(action);
                }
            }

            Parallel.ForEach(checkedActions, action =>
            {
                using var rentedActionsStack = ActionStackPool.Rent(out var actionsStack);
                var boardPool = new Figure[_difficulty][];
                try
                {
                    for (var i = 0; i < _difficulty; i++)
                    {
                        _ = BoardPool<Figure>.Rent(out var board);
                        boardPool[i] = board;
                    }

                    using var rentedClonedBoard = BoardPool<Figure>.Rent(out var clonedBoard);
                    _board.CopyTo(clonedBoard, 0);

                    FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                    var eval = AlphaBeta(clonedBoard, _difficulty - 1, int.MinValue, int.MaxValue, false, actionsStack, boardPool);
                    resultActions.Add((eval, action));
                    // Console.WriteLine($"Evaluated action {sw.Elapsed} action: {action}, eval: {eval}");
                }
                finally
                {
                    for (var i = 0; i < _difficulty; i++)
                    {
                        BoardPool<Figure>.Return(boardPool[i]);
                    }
                }
            });

            var resultArray = resultActions.ToList();
            var alpha = resultArray.Max(x => x.value);
            for (var i = resultActions.Count - 1; i >= 0; i--)
            {
                if (resultArray[i].value < alpha - 0)
                    resultArray.RemoveAt(i);
            }

            var executedAction = resultArray[_random.Next(0, resultArray.Count)];
            Console.WriteLine($"Time for turn: {sw.Elapsed}, evaluation: {executedAction.value}");
            // if (sw.Elapsed < TimeSpan.FromSeconds(1))
                // Thread.Sleep(TimeSpan.FromSeconds(1) - sw.Elapsed);
            _requestMove.Invoke(executedAction.action.SourceIndex, executedAction.action.TargetIndex, TimeSpan.Zero);
        }
        finally
        {
            foreach (var analysisValue in _analysis.Values)
            {
                BoardPool<int>.Return(analysisValue);
            }
        }
    }

    private void HandleWhitePlayer()
    {
        var sw = new Stopwatch();
        sw.Start();

        _analysis = FigureImpactAnalyzer.AnalyzeFigures(_board);
        try
        {
            var resultActions = new ConcurrentBag<(int value, FigureAction action)>();
            using var rentedCheckedActions = ActionStackPool.Rent(out var checkedActions);
            using var rentedCurrentActions = ActionStackPool.Rent(out var currentActions);

            for (var i = 0; i < _board.Length; i++)
            {
                var figure = _board[i];
                if (figure.IsBlack())
                    continue;

                var countBefore = currentActions.Count;
                FigureActionsResolver.GetPossibleActions(i, figure, _board, currentActions);
                var added = currentActions.Count - countBefore;

                for (var j = 0; j < added; j++)
                {
                    var action = currentActions.Pop();
                    if (!action.FigureActionType.IsExecutable())
                        continue;

                    checkedActions.Push(action);
                }
            }

            Parallel.ForEach(checkedActions, action =>
            {
                var boardPool = new Figure[_difficulty][];
                try
                {
                    using var rentedActionsStack = ActionStackPool.Rent(out var actionsStack);
                    for (var i = 0; i < _difficulty; i++)
                    {
                        _ = BoardPool<Figure>.Rent(out var board);
                        boardPool[i] = board;
                    }

                    using var rentedClonedBoard = BoardPool<Figure>.Rent(out var clonedBoard);
                    _board.CopyTo(clonedBoard, 0);

                    FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                    var eval = AlphaBeta(clonedBoard, _difficulty - 1, int.MinValue, int.MaxValue, true, actionsStack, boardPool);
                    resultActions.Add((eval, action));
                    // Console.WriteLine($"Evaluated action {sw.Elapsed} action: {action}, eval: {eval}");
                }
                finally
                {
                    for (var i = 0; i < _difficulty; i++)
                    {
                        BoardPool<Figure>.Return(boardPool[i]);
                    }
                }
            });

            var resultArray = resultActions.ToList();
            var beta = resultArray.Min(x => x.value);
            for (var i = resultActions.Count - 1; i >= 0; i--)
            {
                if (resultArray[i].value > beta + 0)
                    resultArray.RemoveAt(i);
            }

            var executedAction = resultArray[_random.Next(0, resultArray.Count)];
            Console.WriteLine($"Time for turn: {sw.Elapsed}, evaluation: {executedAction.value}");
            // if (sw.Elapsed < TimeSpan.FromSeconds(1))
                // Thread.Sleep(TimeSpan.FromSeconds(1) - sw.Elapsed);
            _requestMove.Invoke(executedAction.action.SourceIndex, executedAction.action.TargetIndex, TimeSpan.Zero);
        }
        finally
        {
            foreach (var analysisValue in _analysis.Values)
            {
                BoardPool<int>.Return(analysisValue);
            }
        }
    }

    private int AlphaBeta(
        Figure[] currentBoard,
        int depth, int alpha, int beta, bool maximizingPlayer,
        Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var check = EvaluateBoard(currentBoard, actionsStack, maximizingPlayer ? Figure.IsBlack : Figure.IsWhite);
        if (depth == 0 || Math.Abs(check) > 10_000_000)
        {
            return check;
        }

        if (maximizingPlayer)
        {
            var maxEval = int.MinValue;
            for (var i = 0; i < currentBoard.Length; i++)
            {
                var figure = currentBoard[i];
                if (figure.IsWhite())
                    continue;

                var oldCount = actionsStack.Count;
                FigureActionsResolver.GetPossibleActions(i, figure, currentBoard, actionsStack);
                var added = actionsStack.Count - oldCount;
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    if (beta <= alpha)
                        continue; // Beta cut-off

                    if (!action.FigureActionType.IsExecutable())
                        continue;

                    var clonedBoard = boardPool[depth];
                    currentBoard.CopyTo(clonedBoard, 0);
                    FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                    var eval = AlphaBeta(clonedBoard, depth - 1, alpha, beta, false, actionsStack, boardPool);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                }
            }
            return maxEval;
        }
        else
        {
            var minEval = int.MaxValue;
            for (var i = 0; i < currentBoard.Length; i++)
            {
                var figure = currentBoard[i];
                if (figure.IsBlack())
                    continue;

                var oldCount = actionsStack.Count;
                FigureActionsResolver.GetPossibleActions(i, figure, currentBoard, actionsStack);
                var added = actionsStack.Count - oldCount;
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    if (beta <= alpha)
                        continue; // Alpha cut-off

                    if (!action.FigureActionType.IsExecutable())
                        continue;

                    var clonedBoard = boardPool[depth];
                    currentBoard.CopyTo(clonedBoard, 0);
                    FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                    var eval = AlphaBeta(clonedBoard, depth - 1, alpha, beta, true, actionsStack, boardPool);
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
    private int EvaluateBoard(ReadOnlySpan<Figure> board, Stack<FigureAction> actionsStack, Figure currentFigureColor)
    {
        var analysis = _analysis;
        var evaluation = 0;
        for (var i = 0; i < board.Length; i++)
        {
            var figure = board[i];
            evaluation += analysis[(int)figure][i];
        }

        if (Math.Abs(evaluation) > 800_000_000)
        {
            return evaluation;
        }

        for (var i = 0; i < board.Length; i++)
        {
            var figure = board[i];
            if (figure.IsNeutral())
            {
                continue;
            }

            var oldCount = actionsStack.Count;
            FigureActionsResolver.GetPossibleActions(i, figure, board, actionsStack);
            var added = actionsStack.Count - oldCount;
            if (figure.IsWhite())
            {
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    evaluation -= ActionImpactEvaluator.EvaluateAction(board, action, currentFigureColor);
                }
            }
            else
            {
                for (var j = 0; j < added; j++)
                {
                    var action = actionsStack.Pop();
                    evaluation += ActionImpactEvaluator.EvaluateAction(board, action, currentFigureColor);
                }
            }
        }
        return evaluation;
    }
}