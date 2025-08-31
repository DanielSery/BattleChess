using BattleChess3.Core;
using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;

namespace BattleChess3.Maps;

internal class FigureCreator : IFigureCreator
{
    private readonly IFigureOwnersHolder _gameService;
    private readonly IFigureService _figureService;

    public FigureCreator(
        IFigureOwnersHolder gameService,
        IFigureService figureService)
    {
        _gameService = gameService;
        _figureService = figureService;
    }

    public Figure CreateFigure(FigureBlueprint figureBlueprint)
    {
        var figureType = _figureService.GetFigureByUniqueUnitId(figureBlueprint.FigureId);
        var player = _gameService.GetFigureOwner(figureBlueprint.Player);
        var figure = new Figure(player, figureType, figureBlueprint.IsKing);
        player.Figures.Add(figure);
        return figure;
    }

    public Figure CreateEmptyFigure()
    {
        var figureIdentifier = new FigureBlueprint(Player.Neutral, 0, false);
        var figureType = _figureService.GetFigureByUniqueUnitId(figureIdentifier.FigureId);
        return new Figure(NeutralFigureOwner.Instance, figureType, figureIdentifier.IsKing);
    }
}