using BattleChess3.Game.Figures;

namespace BattleChess3.Maps;

internal class FigureService : IFigureService
{
    private readonly Dictionary<int, IFigureType> _figuresDictionary;

    public FigureService(IFigureGroup figureGroup)
    {
        FigureGroups = [figureGroup];
        _figuresDictionary = FigureGroups.SelectMany(group => group.FigureTypes)
            .ToDictionary(figure => figure.FigureId, figure => figure);
    }

    public IList<IFigureGroup> FigureGroups { get; }

    public IFigureType GetFigureByUniqueUnitId(int uniqueUnitId)
    {
        return _figuresDictionary[uniqueUnitId];
    }
}