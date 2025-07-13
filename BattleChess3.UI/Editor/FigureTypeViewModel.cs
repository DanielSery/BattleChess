using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.UI.Editor;

public sealed class FigureTypeViewModel
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
            PlayerId = 1;
            ImageUri = redUri;
        }
        else if (figureType.ImageUris.TryGetValue(0, out var neutralUri))
        {
            PlayerId = 0;
            ImageUri = neutralUri;
        }
    }

    public int PlayerId { get;}
    public int FigureId { get; }
    public string DisplayName { get; }
    public string BaseDescription { get; }
    public string MovementDescription { get; }
    public string AttackDescription { get; }
    public string SpecialDescription { get; }
    public bool IsUnlocked { get;}
    public Uri? ImageUri { get; }
}