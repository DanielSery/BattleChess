using System.ComponentModel;
using System.Diagnostics;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

[DebuggerDisplay("{Type.DisplayName}:{Owner.Player}")]
public sealed class Figure : IFigureInfo, INotifyPropertyChanged
{
    public static readonly Figure None = new(LocalPlayerInfo.Neutral, NoneFigureType.Instance, false);

    public Figure(IFigureOwner owner, IFigureType type, bool isKing)
    {
        if (type != NoneFigureType.Instance &&
            !type.ImageUris.ContainsKey(owner.Player.ToInt()))
        {
            throw new ArgumentException("Figure cannot belong to given player");
        }

        Id = Guid.NewGuid();
        Owner = owner;
        Type = type;
        IsKing = isKing;
    }

    public Guid Id { get; }
    public IFigureOwner Owner { get; }
    public IFigureType Type { get; }
    public bool IsKing { get; }

    public int FigureValue => Type.FigureValue;
    public Uri ImageUri => Type.ImageUris[Owner.Player.ToInt()];
    public string DisplayName => Type.DisplayName;
    public string BaseDescription => Type.BaseDescription;
    public string MovementDescription => Type.MovementDescription;
    public string AttackDescription => Type.AttackDescription;
    public string SpecialDescription => Type.SpecialDescription;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return Type.GetPossibleActions(unitTile, board);
    }
}