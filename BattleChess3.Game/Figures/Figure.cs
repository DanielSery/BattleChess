using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using BattleChess3.Game.GameBoard;
using BattleChess3.Game.Helpers;
using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

public sealed class Figure : IFigure, IFigureInfo, INotifyPropertyChanged
{
    public static readonly Figure None = new(PlayerInfo.Neutral, NoneFigureType.Instance, false);

    public Figure(PlayerInfo owner, IFigureType type, bool isKing)
    {
        Debug.Assert(type == NoneFigureType.Instance ||
            type.ImageUris.ContainsKey(owner.Player.ToInt()));

        Id = Guid.NewGuid();
        Owner = owner;
        Type = type;
        IsKing = isKing;
    }

    public Guid Id { get; }
    public PlayerInfo Owner { get; }
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

    public override string ToString()
    {
        return $"{Type.DisplayName}:{Owner.Player}";
    }
}