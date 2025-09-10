using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Maps.Figures;

internal class FigureCreator : IFigureCreator
{
    private readonly IPlayersOwner _figureOwners;
    private readonly IFigureTypeInfoGroup _figureTypeInfoGroup;

    public FigureCreator(
        IPlayersOwner figureOwners,
        IFigureTypeInfoGroup figureTypeInfoGroup)
    {
        _figureOwners = figureOwners;
        _figureTypeInfoGroup = figureTypeInfoGroup;
    }

    public IFigureWithInfo CreateFigure(Figure figureBlueprint)
    {
        var figureType = _figureTypeInfoGroup.GetFigureTypeById(figureBlueprint.FigureType);
        var player = _figureOwners.GetPlayer(figureBlueprint.PlayerColor);
        return new FigureWithInfo(player, figureType, figureBlueprint.IsKing);
    }

    public IFigureWithInfo CreateEmptyFigure()
    {
        const int emptyFigureId = 0;
        var figureType = _figureTypeInfoGroup.GetFigureTypeById(emptyFigureId);
        return new FigureWithInfo(NeutralPlayer.Instance, figureType, false);
    }
}