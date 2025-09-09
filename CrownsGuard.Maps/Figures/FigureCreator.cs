using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Maps.Figures;

internal class FigureCreator : IFigureCreator
{
    private readonly IFigureOwnersHolder _figureOwners;
    private readonly IFigureGroup _figureGroup;

    public FigureCreator(
        IFigureOwnersHolder figureOwners,
        IFigureGroup figureGroup)
    {
        _figureOwners = figureOwners;
        _figureGroup = figureGroup;
    }

    public IFigure CreateFigure(FigureBlueprint figureBlueprint)
    {
        var figureType = _figureGroup.GetFigureTypeById(figureBlueprint.FigureId);
        var player = _figureOwners.GetFigureOwner(figureBlueprint.Player);
        var figure = new FigureInfo(player, figureType, figureBlueprint.IsKing);
        player.Figures.Add(figure);
        return figure;
    }

    public IFigure CreateEmptyFigure()
    {
        const int emptyFigureId = 0;
        var figureType = _figureGroup.GetFigureTypeById(emptyFigureId);
        return new FigureInfo(NeutralFigureOwner.Instance, figureType, false);
    }
}