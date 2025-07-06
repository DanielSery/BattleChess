using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.UI.ViewModel;

public sealed class FigureTypeViewModel
{
    private readonly Func<ITile, IBoard, IEnumerable<FigureAction>> _getActionsFunc;

    public FigureTypeViewModel(IFigureType figureType)
    {
        _getActionsFunc = figureType.GetPossibleActions;

        FigureId = figureType.FigureId;
        DisplayName = figureType.DisplayName;
        BaseDescription = figureType.BaseDescription;
        MovementDescription = figureType.MovementDescription;
        AttackDescription = figureType.AttackDescription;
        SpecialDescription = figureType.SpecialDescription;

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

    public int PlayerId { get; set; }
    public int FigureId { get; }
    public string DisplayName { get; }
    public string BaseDescription { get; }
    public string MovementDescription { get; }
    public string AttackDescription { get; }
    public string SpecialDescription { get; }
    public Uri? ImageUri { get; }
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return _getActionsFunc.Invoke(unitTile, board);
    }
}