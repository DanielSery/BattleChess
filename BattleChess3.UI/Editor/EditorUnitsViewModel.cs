using System.Collections;
using BattleChess3.Game.Figures;
using BattleChess3.Multiplayer;
using CommunityToolkit.Mvvm.Input;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Editor;

public class EditorUnitsViewModel : ViewModelBase, IDisposable
{
    private readonly IFigureService _figureService;
    private readonly IMultiplayerPlayerService _playerService;
    
    private FigureTypeViewModel _tileInfo = new FigureTypeViewModel(Figure.None, false);
    private FigureTypeViewModel[] _figures = [];
    private bool _tileInfoFocused;

    public EditorUnitsViewModel(IFigureService figureService, IMultiplayerPlayerService playerService)
    {
        _figureService = figureService;
        _figureService.FigureGroupsChanged += OnFigureGroupsChanged;
        _playerService = playerService;
        _playerService.LoggedInPlayerChanged += PlayerServiceOnLoggedInPlayerChanged;

        RefreshFigures();

        FigureGotFocusCommand = new RelayCommand<FigureTypeViewModel>(GotFocus);
        FigureLostFocusCommand = new RelayCommand<FigureTypeViewModel>(LostFocus);
        FigureMouseEnterCommand = new RelayCommand<FigureTypeViewModel>(MouseEnterTile);
        FigureMouseExitCommand = new RelayCommand<FigureTypeViewModel>(MouseExitTile);
    }

    public FigureTypeViewModel[] Figures
    {
        get => _figures;
        private set => SetProperty(ref _figures, value);
    }

    public FigureTypeViewModel TileInfo
    {
        get => _tileInfo;
        private set => SetProperty(ref _tileInfo, value);
    }
    
    public RelayCommand<FigureTypeViewModel> FigureGotFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureLostFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureMouseEnterCommand { get; }
    public RelayCommand<FigureTypeViewModel> FigureMouseExitCommand { get; }

    private void GotFocus(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        TileInfo = obj;
        _tileInfoFocused = true;
    }

    private void LostFocus(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        TileInfo = new FigureTypeViewModel(Figure.None, false);
        _tileInfoFocused = false;
    }

    private void MouseExitTile(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        if (_tileInfoFocused)
            return;
        
        TileInfo = new FigureTypeViewModel(Figure.None, false);
    }

    private void MouseEnterTile(FigureTypeViewModel? obj)
    {
        if (obj is null) return;
        if (_tileInfoFocused)
            return;

        TileInfo = obj;
    }

    private void RefreshFigures()
    {
        var unlockedFigures = _playerService.LoggedInPlayer?.UnlockedFigures 
                              ?? IMultiplayerPlayerService.DefaultUnlockedFigures;
        var unlockedFiguresBitArray = new BitArray(unlockedFigures);
        
        Figures = _figureService.GetFigureGroups()
            .SelectMany(x => x.FigureTypes)
            .Select(x => new FigureTypeViewModel(x, unlockedFiguresBitArray.Get(x.FigureId)))
            .ToArray();
    }

    private void PlayerServiceOnLoggedInPlayerChanged(object? sender, EventArgs e)
    {
        RefreshFigures();
    }

    private void OnFigureGroupsChanged(object? sender, IList<IFigureGroup> groups)
    {
        RefreshFigures();
    }

    public void Dispose()
    {
        _figureService.FigureGroupsChanged -= OnFigureGroupsChanged;
    }
}