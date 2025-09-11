using System.Diagnostics;
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
            var sw = new Stopwatch();
            sw.Start();

            var analysis = FigureImpactAnalyzer.AnalyzeFigures(_board);
            var resultActions = new List<(int value, FigureAction action)>();
            var alpha = int.MinValue;
            const int beta = int.MaxValue;
            var depth = _difficulty;
            
            for (var i = 0; i < _board.Length; i++)
            {
                var figure = _board[i];
                if (figure.PlayerColor == PlayerColor.Black)
                {
                    using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), _board);
                    foreach (var action in actions.Span)
                    {
                        if (!action.FigureActionType.IsExecutable())
                            continue;

                        using var clonedBoard = _board.AsSpan().CloneToArrayPoolMemory();
                        FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
                        var eval = AlphaBeta(clonedBoard.Span, depth - 1, alpha, beta, false, analysis);
                        resultActions.Add((eval, action));
                        alpha = Math.Max(alpha, eval);
                    }
                }
            }

            for (var i = resultActions.Count - 1; i >= 0; i--)
            {
                if (resultActions[i].value < alpha - 50)
                    resultActions.RemoveAt(i);
            }

            var executedAction = resultActions[_random.Next(0, resultActions.Count)];
            Console.WriteLine($"Time for turn: {sw.Elapsed}");
            _requestMove.Invoke(executedAction.action.SourcePosition, executedAction.action.TargetPosition, TimeSpan.Zero);
        });
    }
    
    private int AlphaBeta(Span<Figure> board, int depth, int alpha, int beta, bool maximizingPlayer, Dictionary<Figure, int[]> analysis)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board, analysis);
        }

        if (maximizingPlayer)
        {
            var maxEval = int.MinValue;
            for (var i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.PlayerColor == PlayerColor.Black)
                {
                    using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
                    foreach (var action in actions.Span)
                    {
                        if (!action.FigureActionType.IsExecutable())
                            continue;

                        using var clonedBoard = board.CloneToArrayPoolMemory();
                        FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
                        var eval = AlphaBeta(clonedBoard.Span, depth - 1, alpha, beta, false, analysis);
                        if (eval > maxEval)
                        {
                            maxEval = eval;
                        }
                        
                        alpha = Math.Max(alpha, eval);
                        if (beta <= alpha)
                            break; // Beta cut-off
                    }
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            for (var i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.PlayerColor == PlayerColor.White)
                {
                    using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
                    foreach (var action in actions.Span)
                    {
                        if (!action.FigureActionType.IsExecutable())
                            continue;

                        using var clonedBoard = board.CloneToArrayPoolMemory();
                        FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
                        var eval = AlphaBeta(clonedBoard.Span, depth - 1, alpha, beta, true, analysis);
                        if (eval < minEval)
                        {
                            minEval = eval;
                        }
                        
                        beta = Math.Min(beta, eval);
                        if (beta <= alpha)
                            break; // Alpha cut-off
                    }
                }
            }
            
            return minEval;
        }
    }

    private void OnEvent(BoardEvent arg1, Span<Figure> arg2)
    {
    }

    private static int EvaluateBoard(Span<Figure> board, Dictionary<Figure, int[]> analysis)
    {
        var evaluation = 0;
        for (var i = 0; i < board.Length; i++)
        {
            evaluation += analysis[board[i]][i];
        }
        return evaluation;
    }
}