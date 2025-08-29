using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game;

public record WinResult(bool PublishResult, WinType WinType, IPlayerInfo? Won, IPlayerInfo? Lost);

internal class GameService : IGameService
{
    private readonly IPlayerInfo[] _players = new IPlayerInfo[3];

    public GameService()
    {
        _players[0] = LocalHumanPlayerInfo.Neutral;
        _players[1] = new LocalHumanPlayerInfo(Player.White, string.Empty, InfinitePlayerTimer.Instance);
        _players[2] = new LocalHumanPlayerInfo(Player.Black, string.Empty, InfinitePlayerTimer.Instance);

        CurrentPlayerInfo = _players[0];
        WaitingPlayerInfo = _players[1];
    }


    public bool GameRunning { get; private set; }
    public IPlayerInfo CurrentPlayerInfo { get; private set; }
    private IPlayerInfo WaitingPlayerInfo { get; set; }

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    public event EventHandler<WinResult>? PlayerWon;

    public IPlayerInfo GetPlayerInfo(Player player) => _players[player.ToInt()];
    public IPlayerInfo[] GetPlayerInfos() => _players;

    public void StartGame(IPlayerInfo player1, IPlayerInfo player2, Player startingPlayer)
    {
        _players[0] = LocalHumanPlayerInfo.Neutral;
        _players[1] = player1;
        _players[2] = player2;
        
        CurrentPlayerInfo = startingPlayer == player1.Player ? player1 : player2;
        WaitingPlayerInfo = startingPlayer == player1.Player ? player2 : player1;
        GameRunning = true;

        PlayersChanged?.Invoke(this, EventArgs.Empty);
        CurrentPlayerInfo.StartTurn();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StartTurn()
    {
        (CurrentPlayerInfo, WaitingPlayerInfo) = (WaitingPlayerInfo, CurrentPlayerInfo);
        
        CheckCapturedKing(CurrentPlayerInfo, WaitingPlayerInfo);
        CheckCapturedKing(WaitingPlayerInfo, CurrentPlayerInfo);

        if (!GameRunning)
            return;

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
        PlayerWon?.Invoke(this, _players.All(x => x is ILocalHumanPlayerInfo)
            ? new WinResult(false, WinType.Surrender, WaitingPlayerInfo, CurrentPlayerInfo)
            : new WinResult(true, WinType.Surrender, _players[2], _players[1]));
    }

    public void PlayerLost(IPlayerInfo player, WinType winType, bool notifyOther)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(notifyOther, winType, _players[2], _players[1])
            : new WinResult(notifyOther, winType, _players[1], _players[2]));
    }

    public void PlayerWin(IPlayerInfo player, WinType winType, bool publishResult)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(publishResult, winType, _players[1], _players[2])
            : new WinResult(publishResult, winType, _players[2], _players[1]));
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