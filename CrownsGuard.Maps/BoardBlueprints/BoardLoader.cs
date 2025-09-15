using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Core.Players;
using CrownsGuard.Maps.Figures;
using CrownsGuard.Maps.GameBoard;

namespace CrownsGuard.Maps.BoardBlueprints;

internal class BoardLoader : IBoardLoader
{
    private readonly IFigureCreator _figureCreator;

    public BoardLoader(IFigureCreator figureCreator)
    {
        _figureCreator = figureCreator;
    }
    
    public void LoadBoard(IBoardInfo boardInfo, BoardBlueprint map)
    {
        if (boardInfo.Count() != Constants.FullBoardTilesCount) throw new ArgumentException("Full board needs to have 64 tiles");
        if (map.Figures.Length != Constants.FullBoardTilesCount) throw new ArgumentException("Map blueprint needs to have 64 tiles");
        if (map.Figures.Count(x => x.IsKing() && x.IsWhite()) != 1) throw new ArgumentException("Map blueprint needs to have a white king");
        if (map.Figures.Count(x => x.IsKing() && x.IsBlack()) != 1) throw new ArgumentException("Map blueprint needs to have a black king");

        var index = 0;
        foreach (var tile in boardInfo)
        {
            tile.Figure = _figureCreator.CreateFigure(map.Figures[index++]);
        }
    }

    public void LoadTeamBoard(IBoardInfo boardInfo, BoardBlueprint map)
    {
        if (boardInfo.Count() != map.Figures.Length) throw new ArgumentException("Source and target map size must match");
        if (map.Figures.Count(x => x.IsKing() && x.IsWhite()) != 1) throw new ArgumentException("Map blueprint needs to have a white king");
        if (map.Figures.Any(x => x.IsBlack())) throw new ArgumentException("Partial map blueprint cannot have black figure");

        var index = 0;
        foreach (var tile in boardInfo)
        {
            tile.Figure = _figureCreator.CreateFigure(map.Figures[index++]);
        }
    }
}
