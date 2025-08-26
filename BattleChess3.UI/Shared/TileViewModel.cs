using BattleChess3.Game.Figures;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;
using Nicenis.Windows.ViewModels;

namespace BattleChess3.UI.Shared;

public class TileViewModel : ViewModelBase, ITile
{
    private Figure _figure = Figure.None;

    private bool _isMouseOver;
    private bool _isPossibleAttack;
    private bool _isPossibleMove;
    private bool _isPossibleSpecial;
    private bool _isSelected;
    private bool _isBlack;

    private FigureAction _possibleAction = FigureAction.None;

    public TileViewModel(Position position)
    {
        Position = position;
        IsBlack = position.X % 2 == 0 ^ position.Y % 2 == 0;
    }

    public Position Position { get; }
    public Position AbsolutePosition => Position;

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
            IsPossibleAttack = value.ActionType == FigureActionTypes.Attack;
            IsPossibleMove = value.ActionType == FigureActionTypes.Move;
            IsPossibleSpecial = value.ActionType == FigureActionTypes.Special;
        }
    }

    public Figure Figure
    {
        get => _figure;
        set => SetProperty(ref _figure, value);
    }

    public ITile GetPovTile(Player player)
    {
        return new PovTile(this, player);
    }

    /// <inheritdoc />
    public void OnDied()
    {
        Died?.Invoke(this, EventArgs.Empty);
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