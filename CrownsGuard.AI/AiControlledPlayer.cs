using CrownsGuard.AI.Helpers;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Engine.Helpers;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CrownsGuard.AI;

public sealed class AiControlledPlayer : IAutomaticallyControlledPlayer
{
    private readonly Figure[] _board;
    private readonly Action<byte, byte, TimeSpan> _requestMove;
    private readonly int _difficulty;
    private readonly Random _random;
    private FrozenDictionary<int, int[]> _analysis = FrozenDictionary<int, int[]>.Empty;
    private readonly ConcurrentDictionary<long, int>[] _positionCache = new ConcurrentDictionary<long, int>[10];

    public AiControlledPlayer(PlayerColor playerColor, Figure[] board, Action<byte, byte, TimeSpan> requestMove, int difficulty)
    {
        _random = new Random();
        _board = board;
        _requestMove = requestMove;
        _difficulty = difficulty;
        for (var i = 0; i < _positionCache.Length; i++)
        {
            _positionCache[i] = new ConcurrentDictionary<long, int>();
        }
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
        foreach (var positionCache in _positionCache)
        {
            positionCache.Clear();
        }
        
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
                if (!figure.IsBlack())
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
                    Array.Copy(_board, 0, clonedBoard, 0 , _board.Length);

                    FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                    var eval = WhiteSmartAlphaBeta(clonedBoard, _difficulty - 1, int.MinValue, int.MaxValue, actionsStack, boardPool);
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
                if (!figure.IsWhite())
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
                    Array.Copy(_board, 0, clonedBoard, 0 , _board.Length);

                    FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                    var eval = BlackSmartAlphaBeta(clonedBoard, _difficulty - 1, int.MinValue, int.MaxValue, actionsStack, boardPool);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int BlackSmartAlphaBeta(
        Figure[] currentBoard,
        int depth, int alpha, int beta,
        Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var kingEvaluation = FigureArraySimdHelper.SimdSum(currentBoard);
        if (Math.Abs(kingEvaluation) > 800_000_000)
            return kingEvaluation;

        return depth switch
        {
            <= 1 => BlackFinalAlphaBeta(currentBoard, depth, alpha, beta, actionsStack, boardPool),
            <= 2 => BlackAlphaBeta(currentBoard, depth, alpha, beta, actionsStack, boardPool),
            _ => BlackCachingAlphaBeta(currentBoard, depth, alpha, beta, actionsStack, boardPool)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int WhiteSmartAlphaBeta(
        Figure[] currentBoard,
        int depth, int alpha, int beta,
        Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var kingEvaluation = FigureArraySimdHelper.SimdSum(currentBoard);
        if (Math.Abs(kingEvaluation) > 800_000_000)
            return kingEvaluation;

        return depth switch
        {
            <= 1 => WhiteFinalAlphaBeta(currentBoard, depth, alpha, beta, actionsStack, boardPool),
            <= 2 => WhiteAlphaBeta(currentBoard, depth, alpha, beta, actionsStack, boardPool),
            _ => WhiteCachingAlphaBeta(currentBoard, depth, alpha, beta, actionsStack, boardPool)
        };
    }

    private int WhiteCachingAlphaBeta(Figure[] currentBoard, int depth, int alpha, int beta, Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var cache = _positionCache[depth];
        var minEval = int.MaxValue;
        for (var i = 0; i < currentBoard.Length; i++)
        {
            var figure = currentBoard[i];
            if (!figure.IsWhite())
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
                Array.Copy(currentBoard, 0, clonedBoard, 0, currentBoard.Length);
                FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                var boardHash = FigureArraySimdHelper.FastSimdHash64(clonedBoard);
                if (cache.TryGetValue(boardHash, out var eval))
                {
                    // Console.WriteLine($"Cached {depth}");
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                }
                else
                {
                    eval = BlackSmartAlphaBeta(clonedBoard, depth - 1, alpha, beta, actionsStack, boardPool);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    cache[boardHash] = eval;
                }
            }
        }

        return minEval;
    }

    private int WhiteAlphaBeta(Figure[] currentBoard, int depth, int alpha, int beta, Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var minEval = int.MaxValue;
        for (var i = 0; i < currentBoard.Length; i++)
        {
            var figure = currentBoard[i];
            if (!figure.IsWhite())
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
                Array.Copy(currentBoard, 0, clonedBoard, 0, currentBoard.Length);
                FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                var eval = BlackSmartAlphaBeta(clonedBoard, depth - 1, alpha, beta, actionsStack, boardPool);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
            }
        }
            
        return minEval;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private int WhiteFinalAlphaBeta(Figure[] currentBoard, int depth, int alpha, int beta, Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var minEval = int.MaxValue;
        for (var i = 0; i < currentBoard.Length; i++)
        {
            var figure = currentBoard[i];
            if (!figure.IsWhite())
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
                Array.Copy(currentBoard, 0, clonedBoard, 0, currentBoard.Length);
                FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                var eval = EvaluateBoard(currentBoard, actionsStack, Figure.IsWhite);
                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);
            }
        }
            
        return minEval;
    }

    private int BlackCachingAlphaBeta(Figure[] currentBoard, int depth, int alpha, int beta, Stack<FigureAction> actionsStack,
        Figure[][] boardPool)
    {
        var cache = _positionCache[depth];
        var maxEval = int.MinValue;
        for (var i = 0; i < currentBoard.Length; i++)
        {
            var figure = currentBoard[i];
            if (!figure.IsBlack())
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
                Array.Copy(currentBoard, 0, clonedBoard, 0, currentBoard.Length);
                FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                var boardHash = FigureArraySimdHelper.FastSimdHash64(clonedBoard);
                if (cache.TryGetValue(boardHash, out var eval))
                {
                    // Console.WriteLine($"Cached {depth}");
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                }
                else
                {
                    eval = WhiteSmartAlphaBeta(clonedBoard, depth - 1, alpha, beta, actionsStack, boardPool);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    cache[boardHash] = eval;
                }
            }
        }
        return maxEval;
    }

    private int BlackAlphaBeta(Figure[] currentBoard, int depth, int alpha, int beta, 
        Stack<FigureAction> actionsStack, Figure[][] boardPool)
    {
        var maxEval = int.MinValue;
        for (var i = 0; i < currentBoard.Length; i++)
        {
            var figure = currentBoard[i];
            if (!figure.IsBlack())
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
                Array.Copy(currentBoard, 0, clonedBoard, 0, currentBoard.Length);
                FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                var eval = WhiteSmartAlphaBeta(clonedBoard, depth - 1, alpha, beta, actionsStack, boardPool);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
            }
        }
        return maxEval;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private int BlackFinalAlphaBeta(Figure[] currentBoard, int depth, int alpha, int beta, Stack<FigureAction> actionsStack, Figure[][] boardPool)
    {
        var maxEval = int.MinValue;
        for (var i = 0; i < currentBoard.Length; i++)
        {
            var figure = currentBoard[i];
            if (!figure.IsBlack())
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
                Array.Copy(currentBoard, 0, clonedBoard, 0, currentBoard.Length);
                FigureActionExecutor.ExecuteFigureAction(clonedBoard, action, OnEvent);
                var eval = EvaluateBoard(currentBoard, actionsStack, Figure.IsBlack);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);
            }
        }
        return maxEval;
    }

    private static void OnEvent(BoardEvent boardEvent, Span<Figure> board)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private int EvaluateBoard(ReadOnlySpan<Figure> board, Stack<FigureAction> actionsStack, Figure currentFigureColor)
    {
        var kingEvaluation = FigureArraySimdHelper.SimdSum(board);
        if (Math.Abs(kingEvaluation) > 800_000_000)
            return kingEvaluation;

        var analysis = _analysis;
        var evaluation = 0;
        for (var i = 0; i < board.Length; i++)
        {
            var figure = board[i];
            evaluation += analysis[(int)figure][i];
        }

        return evaluation + BoardEvaluationHelper.EvaluateBoard(board, actionsStack, currentFigureColor);
    }
}