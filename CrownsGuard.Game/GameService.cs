using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Game;

public record WinResult(bool PublishResult, WinType WinType, IPlayerInfo? Won, IPlayerInfo? Lost);

internal class GameService : IGameService, IPlayersOwner
{
    private readonly IPlayer[] _figureOwners = new IPlayer[3];

    public GameService()
    {
        _figureOwners[0] = NeutralPlayer.Instance;
        _figureOwners[1] = WhitePlayer = new ControlledPlayerInfo(PlayerColor.White, string.Empty);
        _figureOwners[2] = BlackPlayer = new ControlledPlayerInfo(PlayerColor.Black, string.Empty);

        CurrentPlayerInfo = WhitePlayer;
        WaitingPlayerInfo = BlackPlayer;
    }

    public bool GameRunning { get; private set; }
    public IPlayerInfo CurrentPlayerInfo { get; private set; }
    public IPlayerInfo WaitingPlayerInfo { get; private set; }

    public IPlayerInfo WhitePlayer { get; private set; }
    public IPlayerInfo BlackPlayer { get; private set; }

    public event EventHandler? PlayersChanged;
    public event EventHandler? TurnStarted;
    public event EventHandler? TurnEnded;
    public event EventHandler<WinResult>? PlayerWon;

    public IPlayer GetPlayer(PlayerColor playerColor) => _figureOwners[playerColor.ToInt()];

    public void StartGame(IPlayerInfo player1, IPlayerInfo player2, PlayerColor startingPlayerColor, ArrayPoolMemory<Figure> board)
    {
        _figureOwners[0] = NeutralPlayer.Instance;
        _figureOwners[1] = WhitePlayer = player1;
        _figureOwners[2] = BlackPlayer = player2;
        
        WhitePlayer.UpdateBoard(BoardFlipper.GetFlippedBoard(board));
        BlackPlayer.UpdateBoard(board);
        
        CurrentPlayerInfo = startingPlayerColor == player1.PlayerColor ? player1 : player2;
        WaitingPlayerInfo =startingPlayerColor == player1.PlayerColor ? player2 : player1;
        GameRunning = true;

        PlayersChanged?.Invoke(this, EventArgs.Empty);
        CurrentPlayerInfo.StartTurn();
        TurnStarted?.Invoke(this, EventArgs.Empty);
    }

    public void SyncBoard()
    {
        WaitingPlayerInfo.UpdateBoard(BoardFlipper.GetFlippedBoard(CurrentPlayerInfo.Board));
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
        PlayerWon?.Invoke(this, WhitePlayer is IControlledPlayerInfo && BlackPlayer is IControlledPlayerInfo
            ? new WinResult(false, WinType.Surrender, WaitingPlayerInfo, CurrentPlayerInfo)
            : new WinResult(true, WinType.Surrender, BlackPlayer, WhitePlayer));
    }

    public void PlayerLost(IPlayerInfo player, WinType winType, bool notifyOther)
    {
        GameRunning = false;
        PlayerWon?.Invoke(this, player.PlayerColor == PlayerColor.White
            ? new WinResult(notifyOther, winType, BlackPlayer, WhitePlayer)
            : new WinResult(notifyOther, winType, WhitePlayer, BlackPlayer));
    }

    public void PlayerWin(IPlayerInfo player, WinType winType, bool publishResult)
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
        
        foreach (var figure in CurrentPlayerInfo.Board.Span)
        {
            if (!figure.IsKing)
                continue;
            
            if (figure.PlayerColor == PlayerColor.White)
                whiteHasKing = true;
            
            if (figure.PlayerColor == PlayerColor.Black)
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
            PlayerWin(CurrentPlayerInfo, WinType.CapturedKing, WaitingPlayerInfo.PlayerColor == PlayerColor.White);
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