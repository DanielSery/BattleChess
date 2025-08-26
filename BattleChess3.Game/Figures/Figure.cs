using System.ComponentModel;
using System.Runtime.CompilerServices;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public class Figure : IFigure, IFigureInfo, INotifyPropertyChanged
{
    public static readonly Figure None = new(Player.Neutral, NoneFigureType.Instance, false);

    public Figure(Player owner, IFigureType type, bool isKing)
    {
        Id = Guid.NewGuid();
        Owner = owner;
        Type = type;
        IsKing = isKing;
    }

    public Figure(Guid id, Player owner, IFigureType type, bool isKing)
    {
        Id = id;
        Owner = owner;
        Type = type;
        IsKing = isKing;
    }

    public Guid Id { get; }
    public Player Owner { get; }
    public IFigureType Type { get; }
    public bool IsKing { get; }
    public int FigureValue => Type.FigureValue;
    public Uri ImageUri => Type.ImageUris[Owner.Index];
    public int FigureId => Type.FigureId;
    public string DisplayName => Type.DisplayName;
    public string BaseDescription => Type.BaseDescription;
    public string MovementDescription => Type.MovementDescription;
    public string AttackDescription => Type.AttackDescription;
    public string SpecialDescription => Type.SpecialDescription;
    public IDictionary<int, Uri> ImageUris => Type.ImageUris;

    public void OnDied(ITile unitTile, IBoard board)
    {
        unitTile.OnDied();
    }

    public void OnMoved(ITile from, ITile to, IBoard board)
    {
        to.OnMovedTo();
    }

    public void OnCreated(ITile tile, IBoard board)
    {
        tile.OnCreated();
    }

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return Type.GetPossibleActions(unitTile, board);
    }

    public override string ToString()
    {
        return $"{Type.DisplayName}:{Owner.Index}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}