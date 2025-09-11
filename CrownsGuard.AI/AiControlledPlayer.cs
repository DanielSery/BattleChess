using System.Diagnostics;
using CrownsGuard.Core;
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
    private int _level = 1;

    public AiControlledPlayer(PlayerColor playerColor, Figure[] board, Action<Position, Position, TimeSpan> requestMove)
    {
        _board = board;
        _requestMove = requestMove;
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
            var board = _board;
            var playerColor = PlayerColor;
            
            var maxImpact = int.MinValue;
            var maxAction = new FigureAction(FigureActionType.None, Position.None, Position.None);
 
            for (var i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.PlayerColor == playerColor)
                {
                    using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
                    foreach (var action in actions.Span)
                    {
                        if (action.FigureActionType is FigureActionType.PossibleAttack or FigureActionType.PossibleSpecial or FigureActionType.None)
                            continue;

                        using var clonedBoard = board.CloneToArrayPoolMemory();
                        FigureActionExecutor.ExecuteFigureAction(clonedBoard.Span, action, OnEvent);
                        if (action.FigureActionType is FigureActionType.Special)
                        {
                            Console.WriteLine("Special");
                        }
                        else if (action.FigureActionType is FigureActionType.Attack)
                        {
                            Console.WriteLine("Attack");
                        }
                        
                        var impact = EvaluateBoard(clonedBoard.Span);
                        if (impact > maxImpact)
                        {
                            maxImpact = impact;
                            maxAction = action;
                        }
                    }
                }
            }

            Console.WriteLine($"Time for turn: {sw.Elapsed}");
            _requestMove.Invoke(maxAction.SourcePosition, maxAction.TargetPosition, TimeSpan.Zero);
        });
    }

    private void OnEvent(BoardEvent arg1, Span<Figure> arg2)
    {
    }

    public int EvaluateBoard(Span<Figure> board)
    {
        var evaluation = 0;
        
        for (var i = 0; i < board.Length; i++)
        {
            var figure = board[i];
            if (figure.PlayerColor == PlayerColor.Neutral)
            {
                continue;
            }

            if (figure.PlayerColor == PlayerColor.White)
            {
                if (figure.IsKing) evaluation -= Constants.KingValue;
                evaluation -= figure.FigureType.GetFigureValue() * Constants.FigureValueCoeff;
                
                using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
                foreach (var action in actions.Span)
                {
                    if (action.FigureActionType is FigureActionType.PossibleAttack or FigureActionType.PossibleSpecial)
                        continue;
                        
                    evaluation -= ActionImpactEvaluator.EvaluateAction(board, action, figure);
                }
            }
            else
            {
                if (figure.IsKing) evaluation += Constants.KingValue;
                evaluation += figure.FigureType.GetFigureValue() * Constants.FigureValueCoeff;
                
                using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
                foreach (var action in actions.Span)
                {
                    if (action.FigureActionType is FigureActionType.PossibleAttack or FigureActionType.PossibleSpecial)
                        continue;
                        
                    evaluation += ActionImpactEvaluator.EvaluateAction(board, action, figure);
                }
            }
        }

        return evaluation;
    }
}