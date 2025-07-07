using System.Runtime.InteropServices;
using System.Security;
using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;
using BattleChess3.Maps;
using BattleChess3.Multiplayer;
using BattleChess3.Multiplayer.Tables;
using BattleChess3.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.ViewModel;

public class MultiplayerLobbyViewModel : ViewModelBase
{
    private readonly TeamBoardViewModel _teamBoardViewModel;
    private readonly IMultiplayerLobbyService _multiplayerLobbyService;
    private readonly IMessageShowService _messageShowService;
    private readonly BoardViewModel _boardViewModel;
    private readonly LoginViewModel _loginViewModel;
    
    public MultiplayerLobbyViewModel(
        TeamBoardViewModel teamBoardViewModel,
        IMultiplayerLobbyService multiplayerLobbyService,
        IMessageShowService messageShowService,
        BoardViewModel boardViewModel,
        LoginViewModel loginViewModel)
    {
        _teamBoardViewModel = teamBoardViewModel;
        _multiplayerLobbyService = multiplayerLobbyService;
        _messageShowService = messageShowService;
        _boardViewModel = boardViewModel;
        _loginViewModel = loginViewModel;
        
        CreateLobbyCommand = new AsyncRelayCommand(CreateLobby);
        JoinLobbyCommand = new AsyncRelayCommand(JoinLobby);
    }

    private List<PublicLobbyData> _lobbies = [];
    public List<PublicLobbyData> Lobbies
    {
        get => _lobbies;
        set => SetProperty(ref _lobbies, value);
    }
    
    private PublicLobbyData _selectedRow;
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

    private async Task CreateLobby()
    {
        var myMap = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };
    
        var random = new Random();
        var isHostStarting = random.Next(0, 1) == 1;
        var gameRequestResult = await _multiplayerLobbyService.CreateLobby(
            Name,
            GetPassword(SecurePassword),
            isHostStarting, 
            myMap);

        if (gameRequestResult.IsFailed)
        {
            _messageShowService.ShowMessage(gameRequestResult.Reasons.First().Message);
            return;
        }
        var gameRequest = gameRequestResult.Value;
        
        var gameJoinResult = await _multiplayerLobbyService.WaitForLobbyPlayer(gameRequestResult.Value);
        if (gameJoinResult.IsFailed)
        {
            _messageShowService.ShowMessage(gameJoinResult.Reasons.First().Message);
            return;
        }
        var gameJoin = gameJoinResult.Value;

        var hisMap = GetFigures(gameJoin.Map);
        var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, isHostStarting);
        _boardViewModel.MultiplayerLoadMap(gameRequest.Id, playedMap);
    }

    private async Task JoinLobby()
    {
        var myMap = new MapBlueprint
        {
            Figures = _teamBoardViewModel.Tiles.Select(x => new FigureIdentifier
            {
                PlayerId = x.Figure.Owner.Id,
                FigureId = ((IFigureType)x.Figure).FigureId,
                IsKing = x.Figure.IsKing
            }).ToArray(),
        };
        
        var joinedLobbyResult = await _multiplayerLobbyService.JoinLobby(
            Name,
            GetPassword(SecurePassword),
            myMap);
        if (joinedLobbyResult.IsFailed)
        {
            _messageShowService.ShowMessage(joinedLobbyResult.Reasons.First().Message);
            return;
        }

        var joinedLobby = joinedLobbyResult.Value;
        var hisMap = GetFigures(joinedLobby.Map);
        var playedMap = GetJoinedMapBlueprint(myMap.Figures, hisMap, !joinedLobby.IsHostStarting);
        _boardViewModel.MultiplayerLoadMap(joinedLobby.Id, playedMap);
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

    public void OnActivation()
    {
        Name = _loginViewModel.Name;
        Task.Run(async () =>
        {
            Lobbies = await _multiplayerLobbyService.GetPublicLobbies();
        });
    }
}