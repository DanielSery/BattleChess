using BattleChess3.Game.Players;

namespace BattleChess3.Game.Figures;

internal class FigureCreator : IFigureCreator
{
    private readonly IGameService _gameService;
    private readonly IFigureService _figureService;

    public FigureCreator(
        IGameService gameService,
        IFigureService figureService)
    {
        _gameService = gameService;
        _figureService = figureService;
    }

    public Figure CreateFigure(FigureIdentifier figureIdentifier)
    {
        var figureType = _figureService.GetFigureByUniqueUnitId(figureIdentifier.FigureId);
        var player = _gameService.GetPlayerInfo(figureIdentifier.Player);
        var figure = new Figure(player, figureType, figureIdentifier.IsKing);
        player.Figures.Add(figure);
        return figure;
    }

    public Figure CreateEmptyFigure()
    {
        var figureIdentifier = new FigureIdentifier(0, 0, false);
        var figureType = _figureService.GetFigureByUniqueUnitId(figureIdentifier.FigureId);
        return new Figure(PlayerInfo.Neutral, figureType, figureIdentifier.IsKing);
    }
}