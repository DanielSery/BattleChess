using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using BattleChess3.Multiplayer.Tables;
using BattleChess3.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class MultiplayerViewModel : ViewModelBase
{
    private readonly TeamBoardViewModel _teamBoardViewModel;
    private readonly IMultiplayerLobbyService _multiplayerLobbyService;
    private readonly IMultiplayerRankedService _multiplayerRankedService;
    private readonly INotificationService _notificationService;
    private readonly BoardViewModel _boardViewModel;
    private readonly LoginViewModel _loginViewModel;
    private readonly ILoadingService _loadingService;
    
    public MultiplayerViewModel(
        TeamBoardViewModel teamBoardViewModel,
        IMultiplayerLobbyService multiplayerLobbyService,
        IMultiplayerRankedService multiplayerRankedService,
        INotificationService notificationService,
        BoardViewModel boardViewModel,
        LoginViewModel loginViewModel,
        ILoadingService loadingService)
    {
        _teamBoardViewModel = teamBoardViewModel;
        _multiplayerLobbyService = multiplayerLobbyService;
        _multiplayerRankedService = multiplayerRankedService;
        _notificationService = notificationService;
        _boardViewModel = boardViewModel;
        _loginViewModel = loginViewModel;
        _loadingService = loadingService;
        
        RankedGameCommand = new AsyncRelayCommand(FindRankedGame);
        CreateLobbyCommand = new AsyncRelayCommand(CreateLobby);
        JoinLobbyCommand = new AsyncRelayCommand(JoinLobby);
    }

    private readonly object _lobbyLock = new object();
    private ObservableCollection<PublicLobbyData> _lobbies = [];
    public ObservableCollection<PublicLobbyData> Lobbies
    {
        get => _lobbies;
        set => SetProperty(ref _lobbies, value);
    }
    
    private PublicLobbyData _selectedRow = new PublicLobbyData();
    public PublicLobbyData SelectedRow
    {
        get => _selectedRow;
        set
        {
            SetProperty(ref _selectedRow, value);
            if (_selectedRow is not null)
            {
                Name = _selectedRow.LobbyName;
            }
        }
    }

    private string _name =  string.Empty;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public SecureString SecurePassword { get; set; } = new SecureString();

    public AsyncRelayCommand CreateLobbyCommand { get; }
    public AsyncRelayCommand JoinLobbyCommand { get; }
    public AsyncRelayCommand RankedGameCommand { get; }

    private async Task FindRankedGame()
    {
        using var loadingOperation = _loadingService.StartLoadingOperation("Finding ranked game");
        var myMap = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };

        var request = await _multiplayerRankedService.FindRankedGameAsync(myMap, loadingOperation.CancellationToken);
        if (request.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, request.Reasons.First().Message);
            return;
        }
        var (isHost, gameSearch, gameSearchJoin) = request.Value;
        if (isHost)
        {
            var hisMap = GetFigures(gameSearchJoin.Map);
            var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, gameSearch.IsHostStarting);
            _boardViewModel.MultiplayerLoadMap(MultiplayerGameType.Ranked | MultiplayerGameType.Host, gameSearch.Id, playedMap);
        }
        else
        {
            var hisMap = GetFigures(gameSearch.Map);
            var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, !gameSearch.IsHostStarting);
            _boardViewModel.MultiplayerLoadMap(MultiplayerGameType.Ranked, gameSearch.Id, playedMap);
        }
    }

    private async Task CreateLobby()
    {
        using var loadingOperation = _loadingService.StartLoadingOperation("Creating lobby");
        var myMap = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };
    
        var gameRequestResult = await _multiplayerLobbyService.CreateLobbyAsync(
            Name,
            GetPassword(SecurePassword),
            myMap,
            loadingOperation.CancellationToken);

        if (gameRequestResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, gameRequestResult.Reasons.First().Message);
            return;
        }
        var gameRequest = gameRequestResult.Value;

        loadingOperation.Message = "Waiting for opponent";
        var gameJoinResult = await _multiplayerLobbyService.WaitForLobbyPlayerAsync(gameRequestResult.Value, loadingOperation.CancellationToken);
        if (gameJoinResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, gameJoinResult.Reasons.First().Message);
            return;
        }
        var gameJoin = gameJoinResult.Value;

        var hisMap = GetFigures(gameJoin.Map);
        var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, gameRequest.IsHostStarting);
        _boardViewModel.MultiplayerLoadMap(MultiplayerGameType.Lobby | MultiplayerGameType.Host, gameRequest.Id, playedMap);
    }

    private async Task JoinLobby()
    {
        using var loadingOperation = _loadingService.StartLoadingOperation("Joining lobby");
        var myMap = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };
        
        var joinedLobbyResult = await _multiplayerLobbyService.JoinLobbyAsync(
            Name,
            GetPassword(SecurePassword),
            myMap,
            loadingOperation.CancellationToken);
        if (joinedLobbyResult.IsFailed)
        {
            _notificationService.ShowMessage(ShownMessage.MessageType.Warning, joinedLobbyResult.Reasons.First().Message);
            
            return;
        }

        var joinedLobby = joinedLobbyResult.Value;
        var hisMap = GetFigures(joinedLobby.Map);
        var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, !joinedLobby.IsHostStarting);
        _boardViewModel.MultiplayerLoadMap(MultiplayerGameType.Lobby, joinedLobby.Id, playedMap);
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

    private static MapBlueprint GetJoinedMapBlueprint(FigureIdentifier[] myFigures, FigureIdentifier[] hisFigures, bool amStarting)
    {
        var figures = new FigureIdentifier[64];
        var blueprint = new MapBlueprint
        {
            StartingPlayer = amStarting ? 1 : 2,
            Figures = figures
        };
            
        for (var i = 0; i < myFigures.Length; i++)
        {
            figures[i + 48] = myFigures[i];
        }

        for (var i = 16; i < 48; i++)
        {
            figures[i] = new FigureIdentifier(0, 0, false);
        }

        for (var i = 0; i < hisFigures.Length; i++)
        {
            figures[GetIndexOfOppositePlayer(i + 48)] = hisFigures[i];
        }

        return blueprint;
    }
    
    private static FigureIdentifier[] GetFigures(byte[] map)
    {
        var figures = new FigureIdentifier[16];
        for (var i = 0; i < figures.Length; i++)
        {
            var index = i * 2;
            var playerId = map[index] % 128;
            if (playerId != 0)
                playerId = 3 - playerId;
            
            figures[i] = new FigureIdentifier(
                playerId,
                map[index + 1],
                map[index] / 128 == 1);
        }

        return figures;
    }

    private static Position GetPositionOfOppositePlayer(int index)
    {
        return Position.FromIndex(index)
            .GetPlayerPOVPosition(1);
    }

    private static int GetIndexOfOppositePlayer(int index)
    {
        return GetPositionOfOppositePlayer(index).Index;
    }

    private CancellationTokenSource? _lobbyWatchCancellation;
    public void OnActivation()
    {
        Name = _loginViewModel.Name;
        Task.Run(async () =>
        {
            var lobbies = new ObservableCollection<PublicLobbyData>(await _multiplayerLobbyService.GetPublicLobbiesAsync(CancellationToken.None));
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
}