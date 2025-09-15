using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps.Figures;

namespace CrownsGuard.UI.Editor;

public sealed class FigureTypeViewModel : IFigureInfo
{
    public FigureTypeViewModel(IFigureTypeInfo figureTypeInfo, bool isUnlocked)
    {
        Figure = figureTypeInfo.Figure;
        DisplayName = figureTypeInfo.DisplayName;
        BaseDescription = figureTypeInfo.BaseDescription;
        MovementDescription = figureTypeInfo.MovementDescription;
        AttackDescription = figureTypeInfo.AttackDescription;
        SpecialDescription = figureTypeInfo.SpecialDescription;
        IsUnlocked = isUnlocked;

        if (figureTypeInfo.ImageUris.TryGetValue(1, out var redUri))
        {
            PlayerColor = PlayerColor.White;
            ImageUri = redUri;
        }
        else if (figureTypeInfo.ImageUris.TryGetValue(0, out var neutralUri))
        {
            PlayerColor = PlayerColor.Neutral;
            ImageUri = neutralUri;
        }
    }

    public PlayerColor PlayerColor { get;}
    public Figure Figure { get; }
    public string DisplayName { get; }
    public string BaseDescription { get; }
    public string MovementDescription { get; }
    public string AttackDescription { get; }
    public string SpecialDescription { get; }
    public bool IsUnlocked { get;}
    public Uri? ImageUri { get; }
}