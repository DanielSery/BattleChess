using BattleChess3.Game.Figures;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace BattleChess3.UI.ViewModel;

public sealed class EditorViewModel : ViewModelBase, IDisposable
{
    private readonly IFigureService _figureService;

    private IList<IFigureGroup> _figureGroups = Array.Empty<IFigureGroup>();

    private FigureViewModel _tileInfo = new FigureViewModel(Figure.None);
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

        GotFocusCommand = new RelayCommand<FigureViewModel>(GotFocus);
        LostFocusCommand = new RelayCommand<FigureViewModel>(LostFocus);
        MouseEnterCommand = new RelayCommand<FigureViewModel>(MouseEnterTile);
        MouseExitCommand = new RelayCommand<FigureViewModel>(MouseExitTile);
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

    public FigureViewModel TileInfo
    {
        get => _tileInfo;
        private set => Set(ref _tileInfo, value);
    }

    public RelayCommand<FigureViewModel> GotFocusCommand { get; }
    public RelayCommand<FigureViewModel> LostFocusCommand { get; }
    public RelayCommand<FigureViewModel> MouseEnterCommand { get; }
    public RelayCommand<FigureViewModel> MouseExitCommand { get; }

    private void GotFocus(FigureViewModel obj)
    {
        TileInfo = obj;
        _tileInfoFocused = true;
    }

    private void LostFocus(FigureViewModel obj)
    {
        TileInfo = new FigureViewModel(Figure.None);
        _tileInfoFocused = false;
    }

    private void MouseExitTile(FigureViewModel obj)
    {
        if (_tileInfoFocused)
            return;
        
        TileInfo = new FigureViewModel(Figure.None);
    }

    private void MouseEnterTile(FigureViewModel obj)
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