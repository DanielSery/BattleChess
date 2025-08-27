using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game;

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

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;

    /// <inheritdoc />
    public event EventHandler<(bool notifyOther, WinType winType, PlayerInfo? won, PlayerInfo? lost)>? PlayerWon;

    public PlayerInfo CurrentPlayerInfo { get; private set; }
    private PlayerInfo WaitingPlayerInfo { get; set; }

    public PlayerInfo GetPlayerInfo(Player player) => _players[player.ToInt()];

    public PlayerInfo[] GetPlayerInfos() => _players;

    public void StartGame(PlayerInfo player1, PlayerInfo player2, Player currentPlayer, bool multiplayer, bool hasTimer)
    {
        _players[0] = PlayerInfo.Neutral;
        _players[1] = player1;
        _players[2] = player2;
        
        CurrentPlayerInfo = currentPlayer == player1.Player ? player1 : player2;
        WaitingPlayerInfo = currentPlayer == player1.Player ? player2 : player1;

        IsMultiplayer = multiplayer;
        HasTimer = hasTimer;
        CanMove = currentPlayer == Player.White || !IsMultiplayer;
        IsWaitingForMove = currentPlayer == Player.Black && IsMultiplayer;
        
        PlayersChanged?.Invoke(this, EventArgs.Empty);
        StartTurn();
    }

    public void NextTurn()
    {
        (CurrentPlayerInfo, WaitingPlayerInfo) = (WaitingPlayerInfo, CurrentPlayerInfo);
        CanMove = CurrentPlayerInfo.Player == Player.White || !IsMultiplayer;
        IsWaitingForMove = CurrentPlayerInfo.Player == Player.Black && IsMultiplayer;
        
        CheckCapturedKing(CurrentPlayerInfo, WaitingPlayerInfo);
        CheckCapturedKing(WaitingPlayerInfo, CurrentPlayerInfo);

        if (CanMove || IsWaitingForMove)
        {
            StartTurn();
        }
    }

    public void Surrender()
    {
        if (CanMove || IsWaitingForMove)
        {
            CanMove = false;
            IsWaitingForMove = false;
            PlayerWon?.Invoke(this, IsMultiplayer 
                ? (true, WinType.Surrender, _players[2], _players[1]) 
                : (false, WinType.Surrender, null, null));
        }
    }

    /// <inheritdoc />
    public void PlayerLost(PlayerInfo player, WinType winType, bool notifyOther)
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? (notifyOther, winType, _players[2], _players[1])
            : (notifyOther, winType, _players[1], _players[2]));
    }

    /// <inheritdoc />
    public void PlayerWin(PlayerInfo player, WinType winType, bool notifyOther)
    {
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? (notifyOther, winType, _players[1], _players[2]) 
            : (notifyOther, winType, _players[2], _players[1]));
    }

    private void CheckCapturedKing(PlayerInfo evaluatedPlayerInfo, PlayerInfo otherPlayerInfo)
    {
        if (!CanMove && !IsWaitingForMove)
            return;
        
        if (evaluatedPlayerInfo.Figures.Any(x => x.IsKing))
            return;
        
        CanMove = false;
        IsWaitingForMove = false;
        PlayerWon?.Invoke(this, (otherPlayerInfo.Player == Player.White, WinType.CapturedKing, otherPlayerInfo, evaluatedPlayerInfo));
    }

    private void StartTurn()
    {
        CurrentPlayerInfo.AddTime(TimeSpan.FromSeconds(10));
        CurrentPlayerInfo.StartTurn();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public TimeSpan EndTurn(TimeSpan? forcedTime = null)
    {
        var timeSpent = CurrentPlayerInfo.OnEndingTurn(forcedTime);
        TurnEnded?.Invoke(this, EventArgs.Empty);
        return timeSpent;
    }
}