using CommunityToolkit.Mvvm.Input;
using CrownsGuard.AI;
using CrownsGuard.Core.Players;
using CrownsGuard.Game.Players;
using CrownsGuard.Maps.Utilities;
using CrownsGuard.UI.Shared;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Game;

public class GameSetupViewModel : ViewModelBase
{
    private int _difficulty;
    private bool _randomSetup;
    private readonly BoardViewModel _boardViewModel;
    private readonly MapsViewModel _mapsViewModel;

    public GameSetupViewModel(BoardViewModel boardViewModel,
        MapsViewModel mapsViewModel)
    {
        _boardViewModel = boardViewModel;
        _mapsViewModel = mapsViewModel;

        RequestEndCommand = new RelayCommand(Cancel);
        ChangeSetupOptionCommand = new RelayCommand(ChangeSetupOption);
        StartHotSeatCommand = new RelayCommand(StartHotSeat);
        StartAiGameCommand = new RelayCommand(StartAiGame);
    }

    public event EventHandler? RequestEnd;

    public RelayCommand ChangeSetupOptionCommand { get; }
    public RelayCommand StartHotSeatCommand { get; }
    public RelayCommand StartAiGameCommand { get; }
    public RelayCommand RequestEndCommand { get; }

    public bool RandomSetup
    {
        get => _randomSetup;
        set => SetProperty(ref _randomSetup, value);
    }

    public int Difficulty
    {
        get => _difficulty;
        set => SetProperty(ref _difficulty, value);
    }

    private void ChangeSetupOption()
    {
        RandomSetup = !RandomSetup;
    }

    private void StartAiGame()
    {
        var mapBlueprint = _mapsViewModel.TeamMap.ExtendFor2Players();

        var whitePlayer = new ControlledPlayerInfo(PlayerColor.White, "Red player");
        // var whitePlayer = new AiControlledPlayer(PlayerColor.White, mapBlueprint.Figures, _boardViewModel.RequestPlayMove, Difficulty);
        var blackPlayer = new AiControlledPlayer(PlayerColor.Black, mapBlueprint.Figures, _boardViewModel.RequestPlayMove, Difficulty);

        _boardViewModel.StartLocalGame(mapBlueprint, whitePlayer, blackPlayer);
    }

    private void StartHotSeat()
    {
        var mapBlueprint = _mapsViewModel.TeamMap.ExtendFor2Players();

        var whitePlayer = new ControlledPlayerInfo(PlayerColor.White, "Red player");
        var blackPlayer = new ControlledPlayerInfo(PlayerColor.Black, "Blue player");

        _boardViewModel.StartLocalGame(mapBlueprint, whitePlayer, blackPlayer);
    }

    private void Cancel()
    {
        RequestEnd?.Invoke(this, EventArgs.Empty);
    }
}