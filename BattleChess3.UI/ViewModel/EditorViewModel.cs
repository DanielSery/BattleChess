using BattleChess3.Game.Figures;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class EditorViewModel : ViewModelBase, IDisposable
{
    private readonly IFigureService _figureService;

    private IList<IFigureGroup> _figureGroups = Array.Empty<IFigureGroup>();

    private FigureTypeViewModel _tileInfo = new FigureTypeViewModel(Figure.None);
    private bool _tileInfoFocused;
    private IFigureGroup _selectedFigureGroup = EmptyFigureGroup.Instance;

    public EditorViewModel(IFigureService figureService)
    {
        _figureService = figureService;
        FigureGroups = _figureService
            .GetFigureGroups()
            .Select<IFigureGroup, IFigureGroup>(x => new FigureGroupViewModel(x))
            .ToArray();
        _figureService.FigureGroupsChanged += OnFigureGroupsChanged;

        GotFocusCommand = new RelayCommand<FigureTypeViewModel>(GotFocus);
        LostFocusCommand = new RelayCommand<FigureTypeViewModel>(LostFocus);
        MouseEnterCommand = new RelayCommand<FigureTypeViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<FigureTypeViewModel>(MouseExitTile);
    }

    public IFigureGroup SelectedFigureGroup
    {
        get => _selectedFigureGroup;
        private set => Set(ref _selectedFigureGroup, value);
    }

    public IList<IFigureGroup> FigureGroups
    {
        get => _figureGroups;
        private set
        {
            Set(ref _figureGroups, value);
            if (_figureGroups.All(x => x.DisplayName != _selectedFigureGroup.DisplayName))
            {
                SelectedFigureGroup = _figureGroups.FirstOrDefault()
                                      ?? EmptyFigureGroup.Instance;
            }
        }
    }

    public FigureTypeViewModel TileInfo
    {
        get => _tileInfo;
        private set => Set(ref _tileInfo, value);
    }

    public RelayCommand<FigureTypeViewModel> GotFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> LostFocusCommand { get; }
    public RelayCommand<FigureTypeViewModel> MouseEnterCommand { get; }
    public RelayCommand<FigureTypeViewModel> MouseExitCommand { get; }

    private void GotFocus(FigureTypeViewModel obj)
    {
        TileInfo = obj;
        _tileInfoFocused = true;
    }

    private void LostFocus(FigureTypeViewModel obj)
    {
        TileInfo = new FigureTypeViewModel(Figure.None);
        _tileInfoFocused = false;
    }

    private void MouseExitTile(FigureTypeViewModel obj)
    {
        if (_tileInfoFocused)
            return;
        
        TileInfo = new FigureTypeViewModel(Figure.None);
    }

    private void MouseEnterTile(FigureTypeViewModel obj)
    {
        if (_tileInfoFocused)
            return;

        TileInfo = obj;
    }

    public void Dispose()
    {
        _figureService.FigureGroupsChanged -= OnFigureGroupsChanged;
    }

    private void OnFigureGroupsChanged(object? sender, IList<IFigureGroup> groups)
    {
        FigureGroups = groups
            .Select<IFigureGroup, IFigureGroup>(x => new FigureGroupViewModel(x))
            .ToArray();
    }
}