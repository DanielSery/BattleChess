using BattleChess3.Game.Board;
using BattleChess3.Game.Figures;

namespace BattleChess3.UI.ViewModel;

public sealed class FigureViewModel : IFigureType
{
    private readonly Func<ITile, IBoard, IEnumerable<FigureAction>> _getActionsFunc;

    public FigureViewModel(IFigureType figureType)
    {
        _getActionsFunc = figureType.GetPossibleActions;

        FigureId = figureType.FigureId;
        SetId = figureType.SetId;
        DisplayName = figureType.DisplayName;
        BaseDescription = figureType.BaseDescription;
        MovementDescription = figureType.MovementDescription;
        AttackDescription = figureType.AttackDescription;
        SpecialDescription = figureType.SpecialDescription;
        ImageUris = figureType.ImageUris;
    }

    public int FigureId { get; }
    public int SetId { get; }
    public string DisplayName { get; }
    public string BaseDescription { get; }
    public string MovementDescription { get; }
    public string AttackDescription { get; }
    public string SpecialDescription { get; }
    public IDictionary<int, Uri> ImageUris { get; }
    
    public IEnumerable<FigureAction> GetPossibleActions(ITile unitTile, IBoard board)
    {
        return _getActionsFunc.Invoke(unitTile, board);
    }
}