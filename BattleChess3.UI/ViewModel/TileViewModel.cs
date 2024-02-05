using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;
using GalaSoft.MvvmLight;

namespace BattleChess3.UI.ViewModel;

public class TileViewModel : ViewModelBase, ITileViewModel
{
    public Position Position { get; }

    private bool _isMouseOver;
    public bool IsMouseOver
    {
        get => _isMouseOver;
        set => Set(ref _isMouseOver, value);
    }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => Set(ref _isSelected, value);
    }

    private bool _isPossibleAttack;
    public bool IsPossibleAttack
    {
        get => _isPossibleAttack;
        private set => Set(ref _isPossibleAttack, value);
    }

    private bool _isPossibleMove;
    public bool IsPossibleMove
    {
        get => _isPossibleMove;
        private set => Set(ref _isPossibleMove, value);
    }

    private bool _isPossibleSpecial;
    public bool IsPossibleSpecial
    {
        get => _isPossibleSpecial;
        private set => Set(ref _isPossibleSpecial, value);
    } 

    private FigureAction _possibleAction = FigureAction.None;
    public FigureAction PossibleAction
    {
        get => _possibleAction;
        set
        {
            Set(ref _possibleAction, value);
            IsPossibleAttack = value.ActionType == FigureActionTypes.Attack;
            IsPossibleMove = value.ActionType == FigureActionTypes.Move;
            IsPossibleSpecial = value.ActionType == FigureActionTypes.Special;
        }
    }

    private Figure _figure = Figure.None;
    public Figure Figure
    {
        get => _figure;
        set => Set(ref _figure, value);
    }

    public TileViewModel(Position position)
    {
        Position = position;
    }

    public ITile GetPovTile(Player player)
        => new PlayedTile(this, player);
}
