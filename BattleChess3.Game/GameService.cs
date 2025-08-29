using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game;

public record WinResult(bool PublishResult, WinType WinType, IPlayerInfo? Won, IPlayerInfo? Lost);

internal class GameService : IGameService
{
    public GameService()
    {
        PlayerInfos[0] = NeutralPlayerInfo.Instance;
        PlayerInfos[1] = new LocalPlayerInfo(Player.White, string.Empty);
        PlayerInfos[2] = new LocalPlayerInfo(Player.Black, string.Empty);

        CurrentPlayerInfo = PlayerInfos[0];
        WaitingPlayerInfo = PlayerInfos[1];
    }

    public bool GameRunning { get; private set; }
    public IPlayerInfo CurrentPlayerInfo { get; private set; }
    private IPlayerInfo WaitingPlayerInfo { get; set; }
    public IPlayerInfo[] PlayerInfos { get; } = new IPlayerInfo[3];

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    public event EventHandler<WinResult>? PlayerWon;

    public IPlayerInfo GetPlayerInfo(Player player) => PlayerInfos[player.ToInt()];

    public void StartGame(IPlayerInfo player1, IPlayerInfo player2, Player startingPlayer)
    {
        PlayerInfos[0] = NeutralPlayerInfo.Instance;
        PlayerInfos[1] = player1;
        PlayerInfos[2] = player2;
        
        CurrentPlayerInfo = startingPlayer == player1.Player ? player1 : player2;
        WaitingPlayerInfo = startingPlayer == player1.Player ? player2 : player1;
        GameRunning = true;

        PlayersChanged?.Invoke(this, EventArgs.Empty);
        CurrentPlayerInfo.Timer.StartTurnTimer();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void StartTurn()
    {
        (CurrentPlayerInfo, WaitingPlayerInfo) = (WaitingPlayerInfo, CurrentPlayerInfo);
        
        CheckCapturedKing(CurrentPlayerInfo, WaitingPlayerInfo);
        CheckCapturedKing(WaitingPlayerInfo, CurrentPlayerInfo);

        if (!GameRunning)
            return;

        CurrentPlayerInfo.Timer.StartTurnTimer();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void EndTurn(TimeSpan? forcedTime = null)
    {
        CurrentPlayerInfo.Timer.EndTurnTimer(forcedTime);
        TurnEnded?.Invoke(this, EventArgs.Empty);
    }

    public void Surrender()
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, PlayerInfos.All(x => x is ILocalPlayerInfo)
            ? new WinResult(false, WinType.Surrender, WaitingPlayerInfo, CurrentPlayerInfo)
            : new WinResult(true, WinType.Surrender, PlayerInfos[2], PlayerInfos[1]));
    }

    public void PlayerLost(IPlayerInfo player, WinType winType, bool notifyOther)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(notifyOther, winType, PlayerInfos[2], PlayerInfos[1])
            : new WinResult(notifyOther, winType, PlayerInfos[1], PlayerInfos[2]));
    }

    public void PlayerWin(IPlayerInfo player, WinType winType, bool publishResult)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.Player == Player.White
            ? new WinResult(publishResult, winType, PlayerInfos[1], PlayerInfos[2])
            : new WinResult(publishResult, winType, PlayerInfos[2], PlayerInfos[1]));
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