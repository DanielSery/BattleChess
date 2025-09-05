using System.Collections;
using CrownsGuard.Core.Figures;
using CrownsGuard.Multiplayer;
using CrownsGuard.Multiplayer.Players;
using CommunityToolkit.Mvvm.Input;
using CrownsGuard.UI.Shared;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Editor;

public class EditorUnitsViewModel : ViewModelBase
{
    private readonly IFigureGroup _figureGroup;
    private readonly IMultiplayerPlayerService _playerService;

    private IFigureInfo _mouseOnInfo = new FigureTypeViewModel(NoneFigureType.Instance, false);
    private IFigureInfo _tileInfo = new FigureTypeViewModel(NoneFigureType.Instance, false);
    private FigureTypeViewModel[] _figures = [];
    private bool _tileInfoFocused;

    public EditorUnitsViewModel(
        IFigureGroup figureGroup,
        IMultiplayerPlayerService playerService)
    {
        _figureGroup = figureGroup;
        _playerService = playerService;
        _playerService.LoggedInPlayerChanged += PlayerServiceOnLoggedInPlayerChanged;

        RefreshFigures();

        FigureGotFocusCommand = new RelayCommand<FigureTypeViewModel>(GotFocus);
        FigureLostFocusCommand = new RelayCommand<FigureTypeViewModel>(LostFocus);
        FigureMouseEnterCommand = new RelayCommand<FigureTypeViewModel>(FigureMouseEnter);
        FigureMouseExitCommand = new RelayCommand<FigureTypeViewModel>(FigureMouseExit);
        TileMouseEnterCommand = new RelayCommand<TileViewModel>(TileMouseEnter);
        TileMouseExitCommand = new RelayCommand<TileViewModel>(TileMouseExit);
    }

    public FigureTypeViewModel[] Figures
    {
        get => _figures;
        private set => SetProperty(ref _figures, value);
    }

    public IFigureInfo TileInfo
    {
        get => _tileInfo;
        private set => SetProperty(ref _tileInfo, value);
    }
    
    public RelayCommand<FigureTypeViewModel> FigureGotFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureLostFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureMouseEnterCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureMouseExitCommand { get; }
    public RelayCommand<TileViewModel> TileMouseEnterCommand { get; }
    public RelayCommand<TileViewModel> TileMouseExitCommand { get; }

    public async Task<string?> PossiblyUnlockUnit(bool isWin)
    {
        var random = new Random();
        var chance = isWin ? 0.015 : 0.01;
        var potentialUnlock = new List<(int, string)>();
        
        foreach (var figure in Figures)
        {
            if (figure.IsUnlocked)
                continue;

            if (random.NextDouble() <= chance)
                potentialUnlock.Add((figure.FigureId, figure.DisplayName));
        }
        
        if (potentialUnlock.Count == 0)
            return null;
        
        var unlocked = potentialUnlock[random.Next(0, potentialUnlock.Count)];
        var result = await _playerService.UpdateCurrentPlayerUnlockedFigure(unlocked.Item1, CancellationToken.None);
        if (result.IsFailed)
        {
            return null;
        }
        
        return unlocked.Item2;
    }

    private void GotFocus(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        TileInfo = obj;
        _tileInfoFocused = true;
    }

    private void LostFocus(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        TileInfo = _mouseOnInfo;
        _tileInfoFocused = false;
    }

    private void FigureMouseExit(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        
        _mouseOnInfo = new FigureTypeViewModel(NoneFigureType.Instance, false);
        if (_tileInfoFocused)
            return;
        
        TileInfo = _mouseOnInfo;
    }

    private void FigureMouseEnter(FigureTypeViewModel? obj)
    {
        if (obj is null) return;

        _mouseOnInfo = obj;
        if (_tileInfoFocused)
            return;

        TileInfo = _mouseOnInfo;
    }

    private void TileMouseExit(TileViewModel? obj)
    {
        if (obj is null) return;

        _mouseOnInfo = new FigureTypeViewModel(NoneFigureType.Instance, false);
        if (_tileInfoFocused)
            return;
        
        TileInfo =  _mouseOnInfo;
    }

    private void TileMouseEnter(TileViewModel? obj)
    {
        if (obj is null) return;

        _mouseOnInfo = obj.Figure as IFigureInfo;
        if (_tileInfoFocused)
            return;

        TileInfo = _mouseOnInfo;
    }

    private void RefreshFigures()
    {
        var unlockedFigures = _playerService.LoggedInPlayer?.UnlockedFigures 
                              ?? IMultiplayerPlayerService.DefaultUnlockedFigures;
        var unlockedFiguresBitArray = new BitArray(unlockedFigures);
        
        Figures = _figureGroup.FigureTypes
            .Select(x => new FigureTypeViewModel(x, unlockedFiguresBitArray[x.FigureId]))
            .ToArray();
    }

    private void PlayerServiceOnLoggedInPlayerChanged(object? sender, EventArgs e)
    {
        RefreshFigures();
    }
}