using BattleChess3.Core.Model;
using BattleChess3.Core.Model.Figures;

namespace BattleChess3.UI.ViewModel;

public class FigureViewModel : IFigureType
{
    private readonly Func<ITile, ITile, ITile[], FigureAction> _getActionsFunc;

    public FigureViewModel(IFigureType figureType)
    {
        _getActionsFunc = figureType.GetPossibleAction;

        DisplayName = figureType.DisplayName;
        Description = figureType.Description;
        UnitName = figureType.UnitName;
        ImageUris = figureType.ImageUris;
    }

    public string DisplayName { get; }
    public string Description { get; }
    public string UnitName { get; }
    public IDictionary<int, Uri> ImageUris { get; }

    public FigureAction GetPossibleAction(ITile unitTile, ITile targetTile, ITile[] board)
    {
        return _getActionsFunc.Invoke(unitTile, targetTile, board);
    }
}