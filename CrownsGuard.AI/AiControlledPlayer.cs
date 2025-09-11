using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;
using CrownsGuard.FigureDefinitions.Utilities;
using CrownsGuard.Game.Players;
using CrownsGuard.Game.Timers;

namespace CrownsGuard.AI;

public class AiControlledPlayer : IAutomaticallyControlledPlayerInfo
{
    private readonly Figure[] _board;
    private readonly Action<Position, Position, TimeSpan> _requestMove;

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
            var board = _board;
            var playerColor = PlayerColor;
            
            var maxImpact = 0;
            var maxAction = new FigureAction(FigureActionType.None, Position.None, Position.None);

            for (int i = 0; i < board.Length; i++)
            {
                var figure = board[i];
                if (figure.PlayerColor == playerColor)
                {
                    using var actions = FigureActionsResolver.GetPossibleActions(Position.FromIndex(i), board);
                    foreach (var action in actions.Span)
                    {
                        if (action.FigureActionType is FigureActionType.PossibleAttack or FigureActionType.PossibleSpecial)
                            continue;
                        
                        var impact = ActionImpactEvaluator.EvaluateAction(board, action, figure);
                        if (impact > maxImpact)
                        {
                            maxImpact = impact;
                            maxAction = action;
                        }
                    }
                }
            }
            
            _requestMove.Invoke(maxAction.SourcePosition, maxAction.TargetPosition, TimeSpan.Zero);
        });
    }
}