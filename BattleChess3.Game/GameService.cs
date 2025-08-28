using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game;

public record WinResult(bool PublishResult, WinType WinType, PlayerInfo? Won, PlayerInfo? Lost);

internal class GameService : IGameService
{
    private readonly PlayerInfo[] _players = new PlayerInfo[3];

    public GameService()
    {
        _players[0] = PlayerInfo.Neutral;
        _players[1] = new PlayerInfo(Player.White, string.Empty, null, null);
        _players[2] = new PlayerInfo(Player.Black, string.Empty, null, null);

        CurrentPlayerInfo = _players[0];
        WaitingPlayerInfo = _players[1];
    }

    public bool CanMove { get; private set; }
    public bool IsWaitingForMove { get; private set; }
    public bool HasTimer { get; private set; }
    public bool IsMultiplayer { get; private set; }

    public PlayerInfo CurrentPlayerInfo { get; private set; }
    private PlayerInfo WaitingPlayerInfo { get; set; }

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    public event EventHandler<WinResult>? PlayerWon;

    public PlayerInfo GetPlayerInfo(Player player) => _players[player.ToInt()];
    public PlayerInfo[] GetPlayerInfos() => _players;

    public void StartGame(PlayerInfo player1, PlayerInfo player2, Player startingPlayer, bool multiplayer, bool hasTimer)
    {
        _players[0] = PlayerInfo.Neutral;
        _players[1] = player1;
        _players[2] = player2;
        
        CurrentPlayerInfo = startingPlayer == player1.Player ? player1 : player2;
        WaitingPlayerInfo = startingPlayer == player1.Player ? player2 : player1;

        IsMultiplayer = multiplayer;
        HasTimer = hasTimer;
        CanMove = startingPlayer == Player.White || !IsMultiplayer;
        IsWaitingForMove = startingPlayer == Player.Black && IsMultiplayer;
        
        PlayersChanged?.Invoke(this, EventArgs.Empty);
        StartPlayerClock();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StartTurn()
    {
        (CurrentPlayerInfo, WaitingPlayerInfo) = (WaitingPlayerInfo, CurrentPlayerInfo);
        CanMove = CurrentPlayerInfo.Player == Player.White || !IsMultiplayer;
        IsWaitingForMove = CurrentPlayerInfo.Player == Player.Black && IsMultiplayer;
        
        CheckCapturedKing(CurrentPlayerInfo, WaitingPlayerInfo);
        CheckCapturedKing(WaitingPlayerInfo, CurrentPlayerInfo);

        if (!CanMove && !IsWaitingForMove)
            return;

        StartPlayerClock();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public TimeSpan EndTurn(TimeSpan? forcedTime = null)
    {
        var timeSpent = HasTimer ? CurrentPlayerInfo.OnEndingTurn(forcedTime) : TimeSpan.Zero;
        TurnEnded?.Invoke(this, EventArgs.Empty);
        return timeSpent;
    }

    public void Surrender()
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, IsMultiplayer
            ? new WinResult(true, WinType.Surrender, _players[2], _players[1])
            : new WinResult(false, WinType.Surrender, null, null));
    }

    /// <inheritdoc />
    public void PlayerLost(PlayerInfo player, WinType winType, bool notifyOther)
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(notifyOther, winType, _players[2], _players[1])
            : new WinResult(notifyOther, winType, _players[1], _players[2]));
    }

    /// <inheritdoc />
    public void PlayerWin(PlayerInfo player, WinType winType, bool publishResult)
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(publishResult, winType, _players[1], _players[2])
            : new WinResult(publishResult, winType, _players[2], _players[1]));
    }

    private void CheckCapturedKing(PlayerInfo evaluatedPlayerInfo, PlayerInfo otherPlayerInfo)
    {
        if (!CanMove && !IsWaitingForMove)
            return;
        
        if (evaluatedPlayerInfo.Figures.Any(x => x.IsKing))
            return;
        
        PlayerLost(evaluatedPlayerInfo, WinType.CapturedKing, otherPlayerInfo.Player == Player.White);
    }

    private void StartPlayerClock()
    {
        if (!HasTimer)
            return;

        CurrentPlayerInfo.AddTime(TimeSpan.FromSeconds(10));
        CurrentPlayerInfo.StartTurn();
    }
}