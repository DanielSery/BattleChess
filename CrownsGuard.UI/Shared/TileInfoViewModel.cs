using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Maps.GameBoard;
using Nicenis.Windows.ViewModels;

namespace CrownsGuard.UI.Shared;

public class TileInfoViewModel : ViewModelBase, ITileInfo
{
    public static readonly TileInfoViewModel None = new(new Position(-1, -1));

    private IFigureWithInfo _figure = FigureWithInfo.None;

    private bool _isMouseOver;
    private bool _isPossibleAttack;
    private bool _isPossibleMove;
    private bool _isPossibleSpecial;
    private bool _isSelected;
    private bool _isBlack;

    private FigureAction _possibleAction = FigureAction.None;

    public TileInfoViewModel(Position position)
    {
        Position = position;
        IsBlack = position.X % 2 == 0 ^ position.Y % 2 == 0;
    }

    public Position Position { get; }

    public event EventHandler? MovedFrom;
    public event EventHandler? MovedTo;
    public event EventHandler? Died;
    public event EventHandler? Created;

    public bool IsBlack
    {
        get => _isBlack;
        set => SetProperty(ref _isBlack, value);
    }

    public bool IsMouseOver
    {
        get => _isMouseOver;
        set => SetProperty(ref _isMouseOver, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool IsPossibleAttack
    {
        get => _isPossibleAttack;
        private set => SetProperty(ref _isPossibleAttack, value);
    }

    public bool IsPossibleMove
    {
        get => _isPossibleMove;
        private set => SetProperty(ref _isPossibleMove, value);
    }

    public bool IsPossibleSpecial
    {
        get => _isPossibleSpecial;
        private set => SetProperty(ref _isPossibleSpecial, value);
    }

    public FigureAction PossibleAction
    {
        get => _possibleAction;
        set
        {
            SetProperty(ref _possibleAction, value);
            IsPossibleAttack = value.FigureActionType.HasFlag(FigureActionType.IsAttack);
            IsPossibleMove = value.FigureActionType.HasFlag(FigureActionType.IsMove);
            IsPossibleSpecial = value.FigureActionType.HasFlag(FigureActionType.IsSpecial);
        }
    }

    public IFigureWithInfo Figure
    {
        get => _figure;
        set => SetProperty(ref _figure, value);
    }

    /// <inheritdoc />
    public void OnDied()
    {
        Died?.Invoke(this, EventArgs.Empty);
    }

    public void OnMovedFrom()
    {
        MovedFrom?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void OnMovedTo()
    {
        MovedTo?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void OnCreated()
    {
        Created?.Invoke(this, EventArgs.Empty);
    }
}