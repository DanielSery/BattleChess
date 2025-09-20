using System.ComponentModel;
using System.Diagnostics;
using CrownsGuard.FigureDefinitions.Figures;
using CrownsGuard.Game.Helpers;
using CrownsGuard.Game.Players;

namespace CrownsGuard.Maps.Figures;

[DebuggerDisplay("{TypeInfo.DisplayName}:{Owner.PlayerColor}")]
public sealed class FigureWithInfo : IFigureWithInfo, INotifyPropertyChanged
{
    public static readonly FigureWithInfo None = new(NeutralPlayer.Instance, NoneFigureTypeInfo.Instance, false);

    public FigureWithInfo(IPlayer owner, IFigureTypeInfo typeInfo, bool isKing)
    {
        if (typeInfo != NoneFigureTypeInfo.Instance &&
            !typeInfo.ImageUris.ContainsKey(owner.PlayerColor.ToInt()))
        {
            throw new ArgumentException("Figure cannot belong to given player");
        }

        Owner = owner;
        TypeInfo = typeInfo;
        IsKing = isKing;
    }

    public IPlayer Owner { get; }
    public IFigureTypeInfo TypeInfo { get; }
    public bool IsKing { get; }

    public Uri ImageUri => TypeInfo.ImageUris[Owner.PlayerColor.ToInt()];
    public string DisplayName => TypeInfo.DisplayName;
    public string BaseDescription => TypeInfo.BaseDescription;
    public string MovementDescription => TypeInfo.MovementDescription;
    public string AttackDescription => TypeInfo.AttackDescription;
    public string SpecialDescription => TypeInfo.SpecialDescription;

    public event PropertyChangedEventHandler? PropertyChanged;
}