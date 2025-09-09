using System.ComponentModel;
using System.Diagnostics;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Core.Figures;

[DebuggerDisplay("{Type.DisplayName}:{Owner.Player}")]
public sealed class FigureInfo : IFigure, IFigureInfo, INotifyPropertyChanged
{
    public static readonly FigureInfo None = new(NeutralFigureOwner.Instance, NoneFigureType.Instance, false);

    public FigureInfo(IFigureOwner owner, IFigureType type, bool isKing)
    {
        if (type != NoneFigureType.Instance &&
            !type.ImageUris.ContainsKey(owner.Player.ToInt()))
        {
            throw new ArgumentException("Figure cannot belong to given player");
        }

        Owner = owner;
        Type = type;
        IsKing = isKing;
    }

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
}