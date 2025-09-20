using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Game.Helpers;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Game;

public record WinResult(bool PublishResult, WinType WinType, IPlayer? Won, IPlayer? Lost);

internal class GameService : IGameService, IPlayersOwner
{
    private readonly IPlayer[] _figureOwners = new IPlayer[3];

    public GameService()
    {
        _figureOwners[0] = NeutralPlayer.Instance;
        _figureOwners[1] = WhitePlayer = new ControlledPlayer(PlayerColor.White, string.Empty);
        _figureOwners[2] = BlackPlayer = new ControlledPlayer(PlayerColor.Black, string.Empty);

        CurrentPlayerInfo = WhitePlayer;
        WaitingPlayerInfo = BlackPlayer;
    }

    public bool GameRunning { get; private set; }
    public IPlayer CurrentPlayerInfo { get; private set; }
    public IPlayer WaitingPlayerInfo { get; private set; }

    public IPlayer WhitePlayer { get; private set; }
    public IPlayer BlackPlayer { get; private set; }

    public Figure[] Board { get; private set; } = [];

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    public event EventHandler<WinResult>? PlayerWon;

    public IPlayer GetPlayer(PlayerColor playerColor) => _figureOwners[playerColor.ToInt()];

    public void StartGame(IPlayer player1, IPlayer player2, PlayerColor startingPlayerColor, Figure[] board)
    {
        _figureOwners[0] = NeutralPlayer.Instance;
        _figureOwners[1] = WhitePlayer = player1;
        _figureOwners[2] = BlackPlayer = player2;

        Board = board;
        CurrentPlayerInfo = startingPlayerColor == player1.PlayerColor ? player1 : player2;
        WaitingPlayerInfo =startingPlayerColor == player1.PlayerColor ? player2 : player1;
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
        CheckCapturedKings();
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
        PlayerWon?.Invoke(this, WhitePlayer is IControlledPlayer && BlackPlayer is IControlledPlayer
            ? new WinResult(false, WinType.Surrender, WaitingPlayerInfo, CurrentPlayerInfo)
            : new WinResult(true, WinType.Surrender, BlackPlayer, WhitePlayer));
    }

    public void StopGame()
    {
        GameRunning = false;
    }

    public void PlayerLost(IPlayer player, WinType winType, bool notifyOther)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.PlayerColor == PlayerColor.White
            ? new WinResult(notifyOther, winType, BlackPlayer, WhitePlayer)
            : new WinResult(notifyOther, winType, WhitePlayer, BlackPlayer));
    }

    public void PlayerWin(IPlayer player, WinType winType, bool publishResult)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.PlayerColor == PlayerColor.White
            ? new WinResult(publishResult, winType, WhitePlayer, BlackPlayer)
            : new WinResult(publishResult, winType, BlackPlayer, WhitePlayer));
    }

    private (bool whiteHasKing, bool blackHasKing) CheckKings()
    {
        var whiteHasKing = false;
        var blackHasKing = false;
        
        foreach (var figure in Board)
        {
            if (!figure.IsKing())
                continue;
            
            if (figure.IsWhite())
                whiteHasKing = true;
            
            if (figure.IsBlack())
                blackHasKing = true;
        }
        
        return (whiteHasKing, blackHasKing);
    }

    private void CheckCapturedKings()
    {
        if (!GameRunning)
            return;
        
        var (whiteHasKing, blackHasKing) = CheckKings();
        if (!whiteHasKing && !blackHasKing)
        {
            PlayerWin(WaitingPlayerInfo, WinType.CapturedKing, WaitingPlayerInfo.PlayerColor == PlayerColor.White);
        }
        else if (!whiteHasKing)
        {
            PlayerWin(BlackPlayer, WinType.CapturedKing, WaitingPlayerInfo.PlayerColor == PlayerColor.White);
        }
        else if (!blackHasKing)
        {
            PlayerWin(WhitePlayer, WinType.CapturedKing, WaitingPlayerInfo.PlayerColor == PlayerColor.White);
        }
    }
}