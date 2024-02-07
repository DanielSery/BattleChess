using BattleChess3.Core.Model.Figures;

namespace BattleChess3.UI.ViewModel;

public class FigureGroupViewModel : IFigureGroup
{
    public FigureGroupViewModel(IFigureGroup figureGroup)
    {
        DisplayName = figureGroup.DisplayName;
        FigureTypes = figureGroup.FigureTypes.Select(x => new FigureViewModel(x)).ToArray();
    }

    public string DisplayName { get; }
    public IFigureType[] FigureTypes { get; }
}