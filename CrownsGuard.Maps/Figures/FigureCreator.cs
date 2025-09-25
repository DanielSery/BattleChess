using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.FiguresDesign;
using CrownsGuard.Game;
using CrownsGuard.Game.Players;

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
        var figureType = _figureTypeInfoGroup.GetFigureTypeById(figureBlueprint.GetFigureType());
        var player = _figureOwners.GetPlayer(figureBlueprint.GetFigureColor() switch
        {
            Figure.IsWhite => PlayerColor.White,
            Figure.IsBlack => PlayerColor.Black,
            _ => PlayerColor.Neutral,
        });
        return new FigureWithInfo(player, figureType, figureBlueprint.IsKing());
    }

    public IFigureWithInfo CreateEmptyFigure()
    {
        const int emptyFigureId = 0;
        var figureType = _figureTypeInfoGroup.GetFigureTypeById(emptyFigureId);
        return new FigureWithInfo(NeutralPlayer.Instance, figureType, false);
    }
}