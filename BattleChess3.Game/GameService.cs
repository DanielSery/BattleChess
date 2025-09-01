using BattleChess3.Core;
using BattleChess3.Core.Helpers;
using BattleChess3.Core.Players;
using BattleChess3.Game.Players;

namespace BattleChess3.Game;

public record WinResult(bool PublishResult, WinType WinType, IPlayerInfo? Won, IPlayerInfo? Lost);

internal class GameService : IGameService, IFigureOwnersHolder
{
    private readonly IFigureOwner[] _figureOwners = new IFigureOwner[3];

    public GameService()
    {
        _figureOwners[0] = NeutralFigureOwner.Instance;
        _figureOwners[1] = WhitePlayer = new ControlledPlayerInfo(Player.White, string.Empty);
        _figureOwners[2] = BlackPlayer = new ControlledPlayerInfo(Player.Black, string.Empty);

        CurrentPlayerInfo = WhitePlayer;
        WaitingPlayerInfo = BlackPlayer;
    }

    public bool GameRunning { get; private set; }
    public IPlayerInfo CurrentPlayerInfo { get; private set; }
    private IPlayerInfo WaitingPlayerInfo { get; set; }

    public IPlayerInfo WhitePlayer { get; private set; }
    public IPlayerInfo BlackPlayer { get; private set; }

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    public event EventHandler<WinResult>? PlayerWon;

    public IFigureOwner GetFigureOwner(Player player) => _figureOwners[player.ToInt()];

    public void StartGame(IPlayerInfo player1, IPlayerInfo player2, Player startingPlayer)
    {
        _figureOwners[0] = NeutralFigureOwner.Instance;
        _figureOwners[1] = WhitePlayer = player1;
        _figureOwners[2] = BlackPlayer = player2;
        
        CurrentPlayerInfo = startingPlayer == player1.Player ? player1 : player2;
        WaitingPlayerInfo = startingPlayer == player1.Player ? player2 : player1;
        GameRunning = true;

        PlayersChanged?.Invoke(this, EventArgs.Empty);
        CurrentPlayerInfo.StartTurn();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StartTurn()
    {
        if (!GameRunning)
            return;

        (CurrentPlayerInfo, WaitingPlayerInfo) = (WaitingPlayerInfo, CurrentPlayerInfo);
        
        CheckCapturedKing(CurrentPlayerInfo, WaitingPlayerInfo);
        CheckCapturedKing(WaitingPlayerInfo, CurrentPlayerInfo);

        CurrentPlayerInfo.StartTurn();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void EndTurn(TimeSpan? forcedTime = null)
    {
        CurrentPlayerInfo.EndTurn(forcedTime);
        TurnEnded?.Invoke(this, EventArgs.Empty);
    }

    public void Surrender()
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, WhitePlayer is IControlledPlayerInfo && BlackPlayer is IControlledPlayerInfo
            ? new WinResult(false, WinType.Surrender, WaitingPlayerInfo, CurrentPlayerInfo)
            : new WinResult(true, WinType.Surrender, BlackPlayer, WhitePlayer));
    }

    public void PlayerLost(IPlayerInfo player, WinType winType, bool notifyOther)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(notifyOther, winType, BlackPlayer, WhitePlayer)
            : new WinResult(notifyOther, winType, WhitePlayer, BlackPlayer));
    }

    public void PlayerWin(IPlayerInfo player, WinType winType, bool publishResult)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(publishResult, winType, WhitePlayer, BlackPlayer)
            : new WinResult(publishResult, winType, BlackPlayer, WhitePlayer));
    }

    private void CheckCapturedKing(IPlayerInfo evaluatedPlayerInfo, IPlayerInfo otherPlayerInfo)
    {
        if (!GameRunning)
            return;
        
        if (evaluatedPlayerInfo.Figures.Any(x => x.IsKing))
            return;
        
        PlayerLost(evaluatedPlayerInfo, WinType.CapturedKing, otherPlayerInfo.Player == Player.White);
    }
}