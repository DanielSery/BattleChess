using CrownsGuard.Core;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.GameBoard;
using CrownsGuard.Core.Players;

namespace CrownsGuard.Maps.Utilities;

public static class MapExtender
{
    public static BoardBlueprint ExtendFor2Players(this BoardBlueprint map)
    {
        if (map.Figures.Length != 16) throw new ArgumentException("Partial map blueprint needs to have 16 tiles");
        if (map.Figures.Count(x => x is { IsKing: true, PlayerColor: PlayerColor.White }) != 1) throw new ArgumentException("Map blueprint needs to have a white king");
        if (map.Figures.Any(x => x is { PlayerColor: PlayerColor.Black })) throw new ArgumentException("Partial map blueprint cannot have black figure");

        var resultMap = new BoardBlueprint
        {
            Figures = new Figure[Constants.FullBoardTilesCount],
            StartingPlayerColor = map.StartingPlayerColor
        };

        for (var i = 0; i < map.Figures.Length; i++)
        {
            var whiteFigure = map.Figures[i];
            var whitePosition = Position.FromIndex(i + 64 - 16);
            resultMap.Figures[whitePosition.GetIndex()] = whiteFigure;

            if (whiteFigure.PlayerColor == PlayerColor.White)
            {
                var blackPosition = new Position(whitePosition.X, (sbyte)(7 - whitePosition.Y));
                resultMap.Figures[blackPosition.GetIndex()] = new Figure(PlayerColor.Black, whiteFigure.IsKing, whiteFigure.FigureType);
            }
            else
            {
                var blackPosition = new Position(whitePosition.X, (sbyte)(7 - whitePosition.Y));
                resultMap.Figures[blackPosition.GetIndex()] = new Figure(PlayerColor.Neutral, whiteFigure.IsKing, whiteFigure.FigureType);
            }
        }

        for (var i = 16; i < 48; i++)
        {
            resultMap.Figures[i] = new Figure(PlayerColor.Neutral, false, FigureId.Empty);
        }

        return resultMap;
    }
}