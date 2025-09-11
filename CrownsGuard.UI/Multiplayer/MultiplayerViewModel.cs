using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.Multiplayer.Game;
using CrownsGuard.Multiplayer.Lobby;
using CrownsGuard.Multiplayer.Players;
using CrownsGuard.Multiplayer.Ranked;
using CrownsGuard.Multiplayer.Utilities;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.Core;
using CrownsGuard.Database.Lobby;
using CrownsGuard.UI.Editor;
using CrownsGuard.UI.Game;
using CrownsGuard.UI.Services;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Multiplayer;

public class MultiplayerViewModel : ViewModelBase
{
    private readonly TeamBoardViewModel _teamBoardViewModel;
    private readonly IMultiplayerLobbyService _multiplayerLobbyService;
    private readonly IMultiplayerRankedService _multiplayerRankedService;
    private readonly INotificationService _notificationService;
    private readonly IMultiplayerPlayerService _multiplayerPlayerService;
    private readonly BoardViewModel _boardViewModel;
    private readonly LoginViewModel _loginViewModel;
    private readonly ILoadingService _loadingService;
    private readonly ISoundService _soundService;
    
    public MultiplayerViewModel(
        TeamBoardViewModel teamBoardViewModel,
        IMultiplayerLobbyService multiplayerLobbyService,
        IMultiplayerRankedService multiplayerRankedService,
        INotificationService notificationService,
        IMultiplayerPlayerService multiplayerPlayerService,
        BoardViewModel boardViewModel,
        LoginViewModel loginViewModel,
        ILoadingService loadingService,
        ISoundService soundService)
    {
        _teamBoardViewModel = teamBoardViewModel;
        _multiplayerLobbyService = multiplayerLobbyService;
        _multiplayerRankedService = multiplayerRankedService;
        _notificationService = notificationService;
        _multiplayerPlayerService = multiplayerPlayerService;
        _boardViewModel = boardViewModel;
        _loginViewModel = loginViewModel;
        _loadingService = loadingService;
        _soundService = soundService;
        
        RankedGameCommand = new AsyncRelayCommand(FindRankedGame);
        CreateLobbyCommand = new AsyncRelayCommand(CreateLobby);
        JoinLobbyCommand = new AsyncRelayCommand(JoinLobby);
        RequestEndCommand = new RelayCommand(RaiseRequestEnd);
        EnterCommand = new AsyncRelayCommand(HandleEnterAsync);
    }

    private readonly Lock _lobbyLock = new Lock();
    private ObservableCollection<PublicLobbyData> _lobbies = [];
    public ObservableCollection<PublicLobbyData> Lobbies
    {
        get => _lobbies;
        set => SetProperty(ref _lobbies, value);
    }
    
    private PublicLobbyData? _selectedRow;
    public PublicLobbyData? SelectedRow
    {
        get => _selectedRow;
        set
        {
            SetProperty(ref _selectedRow, value);
            if (value is not null)
            {
                SetProperty(ref _name, value.LobbyName, nameof(Name));
            }
        }
    }

    private string _name =  string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            SetProperty(ref _selectedRow, Lobbies.FirstOrDefault(x => x.LobbyName == value), nameof(SelectedRow));
        }
    }

    public SecureString SecurePassword { get; set; } = new SecureString();

    public AsyncRelayCommand CreateLobbyCommand { get; }
    public AsyncRelayCommand JoinLobbyCommand { get; }
    public AsyncRelayCommand RankedGameCommand { get; }
    public RelayCommand RequestEndCommand { get; }
    public AsyncRelayCommand EnterCommand { get; }

    public event EventHandler? RequestEnd;

    private async Task FindRankedGame()
    {
        using var loadingOperation = _loadingService.StartLoadingOperation("Finding ranked game");
        var myMap = _teamBoardViewModel.GetMapBlueprint();
        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures ?? UnlockedFigures.DefaultUnlockedFigures;
        if (!myMap.IsValid(unlockedFigures))
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Setup has units which weren't unlocked yet");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        _soundService.PlaySoundEffect(SoundEffectType.Button);
        var request = await _multiplayerRankedService.FindRankedGameAsync(myMap, loadingOperation.CancellationToken);
        if (request.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, request.Reasons.First().Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        var (isHost, gameSearch, gameSearchJoin) = request.Value;
        if (isHost)
        {
            var player = _multiplayerPlayerService.GetCurrentPlayer();
            var player2Request = await _multiplayerPlayerService.GetRemotePlayerAsync(gameSearchJoin.PlayerId!, loadingOperation.CancellationToken);
            var player2 = player2Request.IsSuccess
                ? player2Request.Value
                : new RemoteOnlinePlayerInfo(PlayerColor.Black, "Blue player", null, null);
                
            var hisMap = GetFigures(gameSearchJoin.Map);
            var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, gameSearch.IsHostStarting);
            _boardViewModel.StartMultiplayerGame(
                MultiplayerGameType.Ranked | MultiplayerGameType.Host, gameSearch.Id,
                player, player2,
                playedMap, true);
        }
        else
        {
            var player1 = _multiplayerPlayerService.GetCurrentPlayer();
            var player2Request = await _multiplayerPlayerService.GetRemotePlayerAsync(gameSearch.PlayerId!, loadingOperation.CancellationToken);
            var player2 = player2Request.IsSuccess
                ? player2Request.Value
                : new RemoteOnlinePlayerInfo(PlayerColor.Black, "Blue player", null, null);
            
            var hisMap = GetFigures(gameSearch.Map);
            var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, !gameSearch.IsHostStarting);
            _boardViewModel.StartMultiplayerGame(
                MultiplayerGameType.Ranked, gameSearch.Id, 
                player1, player2,
                playedMap, true);
        }
    }

    private async Task CreateLobby()
    {
        using var loadingOperation = _loadingService.StartLoadingOperation("Creating lobby");
        var myMap = _teamBoardViewModel.GetMapBlueprint();
        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures ?? UnlockedFigures.DefaultUnlockedFigures;
        if (!myMap.IsValid(unlockedFigures))
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Setup has units which weren't unlocked yet");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
       
        _soundService.PlaySoundEffect(SoundEffectType.Button); 
        var jobbyResult = await _multiplayerLobbyService.CreateLobbyAsync(
            Name,
            GetPassword(SecurePassword),
            myMap,
            loadingOperation.CancellationToken);

        if (jobbyResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, jobbyResult.Reasons.First().Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        var lobby = jobbyResult.Value;

        loadingOperation.Message = "Waiting for opponent";
        var gameJoinResult = await _multiplayerLobbyService.WaitForLobbyPlayerAsync(jobbyResult.Value, loadingOperation.CancellationToken);
        if (gameJoinResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, gameJoinResult.Reasons.First().Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        var gameJoin = gameJoinResult.Value;
        
        var player1 = _multiplayerPlayerService.GetCurrentPlayer();
        var player2Request = await _multiplayerPlayerService.GetRemotePlayerAsync(gameJoin.PlayerId!, loadingOperation.CancellationToken);
        var player2 = player2Request.IsSuccess
            ? player2Request.Value
            : new RemoteOnlinePlayerInfo(PlayerColor.Black, "Blue player", null, null);

        var hisMap = GetFigures(gameJoin.Map);
        var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, lobby.IsHostStarting);
        _boardViewModel.StartMultiplayerGame(
            MultiplayerGameType.Lobby | MultiplayerGameType.Host, lobby.Id, 
            player1, player2,
            playedMap, false);
    }

    private async Task JoinLobby()
    {
        using var loadingOperation = _loadingService.StartLoadingOperation("Joining lobby");
        var myMap = _teamBoardViewModel.GetMapBlueprint();
        var unlockedFigures = _multiplayerPlayerService.LoggedInPlayer?.UnlockedFigures ?? UnlockedFigures.DefaultUnlockedFigures;
        if (!myMap.IsValid(unlockedFigures))
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, "Setup has units which weren't unlocked yet");
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        
        _soundService.PlaySoundEffect(SoundEffectType.Button); 
        var lobbyResult = await _multiplayerLobbyService.JoinLobbyAsync(
            Name,
            GetPassword(SecurePassword),
            myMap,
            loadingOperation.CancellationToken);
        if (lobbyResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, lobbyResult.Reasons.First().Message);
            _soundService.PlaySoundEffect(SoundEffectType.Error);
            return;
        }
        var lobby = lobbyResult.Value;
        
        var player1 = _multiplayerPlayerService.GetCurrentPlayer();
        var player2Request = await _multiplayerPlayerService.GetRemotePlayerAsync(lobby.PlayerId!, loadingOperation.CancellationToken);
        var player2 = player2Request.IsSuccess
            ? player2Request.Value
            : new RemoteOnlinePlayerInfo(PlayerColor.Black, "Blue player", null, null);
        
        var hisMap = GetFigures(lobby.Map);
        var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, !lobby.IsHostStarting);
        _boardViewModel.StartMultiplayerGame(
            MultiplayerGameType.Lobby, lobby.Id, 
            player1, player2,
            playedMap, false);
    }

    private static string GetPassword(SecureString secureString)
    {
        ArgumentNullException.ThrowIfNull(secureString);

        var unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            return Marshal.PtrToStringUni(unmanagedString)!;
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString); // Clear memory
        }
    }

    private static BoardBlueprint GetJoinedMapBlueprint(Figure[] myFigures, Figure[] hisFigures, bool amStarting)
    {
        var figures = new Figure[64];
        var blueprint = new BoardBlueprint
        {
            StartingPlayerColor = amStarting ? PlayerColor.White : PlayerColor.Black,
            Figures = figures
        };
            
        for (var i = 0; i < myFigures.Length; i++)
        {
            figures[i + 48] = myFigures[i];
        }

        for (var i = 16; i < 48; i++)
        {
            figures[i] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }

        for (var i = 0; i < hisFigures.Length; i++)
        {
            figures[GetIndexOfOppositePlayer(i + 48)] = hisFigures[i];
        }

        return blueprint;
    }
    
    private static Figure[] GetFigures(int[] map)
    {
        var figures = new Figure[16];
        for (var i = 0; i < figures.Length; i++)
        {
            var figure = FigureSerializationHelper.FromInt(map[i]);
            if (figure.PlayerColor == PlayerColor.White)
                figure = new Figure(PlayerColor.Black, figure.IsKing, figure.FigureType);

            figures[i] = figure;
        }

        return figures;
    }

    private static Position GetPositionOfOppositePlayer(int index)
    {
        var position = Position.FromIndex(index);
        return new Position(position.X, (sbyte)(Constants.BoardLength - position.Y - 1));
    }

    private static int GetIndexOfOppositePlayer(int index)
    {
        return GetPositionOfOppositePlayer(index).GetIndex();
    }

    private CancellationTokenSource? _lobbyWatchCancellation;
    public void OnActivation()
    {
        Name = _loginViewModel.Name;
        Task.Run(async () =>
        {
            var lobbies = new ObservableCollection<PublicLobbyData>(
                (await _multiplayerLobbyService.GetPublicLobbiesAsync(CancellationToken.None)).Value);
            Application.Current.Dispatcher.Invoke(() =>
            {
                lock (_lobbyLock)
                {
                    Lobbies = lobbies;
                }
            });
            
            _lobbyWatchCancellation = new CancellationTokenSource();
            await _multiplayerLobbyService.WatchLobbiesAsync(
                OnLobbyAdded, 
                OnLobbyChanged,
                OnLobbyRemoved,
                _lobbyWatchCancellation.Token);
        });
    }

    private void OnLobbyAdded(PublicLobbyData lobbyData)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (!string.IsNullOrEmpty(lobbyData.JoinedId))
                return;

            lock (_lobbyLock)
            {
                Lobbies.Add(lobbyData);
            }
        });
    }

    private Task HandleEnterAsync()
    {
        if (SelectedRow is null)
        {
            return CreateLobby();
        }
        else
        {
            return JoinLobby();
        }
    }

    private void OnLobbyChanged(PublicLobbyData lobbyData)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            lock (_lobbyLock)
            {
                for (var i = 0; i < Lobbies.Count; i++)
                {
                    var lobby = Lobbies[i];
                    if (lobby.Id != lobbyData.Id)
                    {
                        continue;
                    }
                    
                    if (!string.IsNullOrEmpty(lobbyData.JoinedId))
                    {
                        Lobbies.RemoveAt(i);
                    }
                    else
                    {
                        Lobbies[i] = lobbyData;
                    }
                    return;
                }
            }
        });
    }

    private void OnLobbyRemoved(string lobbyId)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            lock (_lobbyLock)
            {
                for (var i = 0; i < Lobbies.Count; i++)
                {
                    var lobby = Lobbies[i];
                    if (lobby.Id == lobbyId)
                    {
                        Lobbies.RemoveAt(i);
                        return;
                    }
                }
            }
        });
    }

    public void OnDeactivation()
    {
        _lobbyWatchCancellation?.Cancel();
        _lobbyWatchCancellation = null;
    }

    private void RaiseRequestEnd()
    {
        RequestEnd?.Invoke(this, EventArgs.Empty);
    }
}