using BattleChess3.Core;
using BattleChess3.Core.Figures;
using BattleChess3.Core.Players;

namespace BattleChess3.Maps.Figures;

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
        var figure = new Figure(player, figureType, figureBlueprint.IsKing);
        player.Figures.Add(figure);
        return figure;
    }

    public IFigure CreateEmptyFigure()
    {
        const int emptyFigureId = 0;
        var figureType = _figureGroup.GetFigureTypeById(emptyFigureId);
        return new Figure(NeutralFigureOwner.Instance, figureType, false);
    }
}