using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;

namespace BattleChess3.UI.Editor;

public sealed class FigureTypeViewModel : IFigureInfo
{
    public FigureTypeViewModel(IFigureType figureType, bool isUnlocked)
    {
        FigureId = figureType.FigureId;
        DisplayName = figureType.DisplayName;
        BaseDescription = figureType.BaseDescription;
        MovementDescription = figureType.MovementDescription;
        AttackDescription = figureType.AttackDescription;
        SpecialDescription = figureType.SpecialDescription;
        IsUnlocked = isUnlocked;

        if (figureType.ImageUris.TryGetValue(1, out var redUri))
        {
            Player = Player.White;
            ImageUri = redUri;
        }
        else if (figureType.ImageUris.TryGetValue(0, out var neutralUri))
        {
            Player = Player.Neutral;
            ImageUri = neutralUri;
        }
    }

    public Player Player { get;}
    public int FigureId { get; }
    public string DisplayName { get; }
    public string BaseDescription { get; }
    public string MovementDescription { get; }
    public string AttackDescription { get; }
    public string SpecialDescription { get; }
    public bool IsUnlocked { get;}
    public Uri? ImageUri { get; }
}