using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Maps.Figures;

namespace BattleChess3.Maps.BoardBlueprints;

internal class BoardBlueprintLoader : IBoardBlueprintLoader
{
    private readonly IFigureCreator _figureCreator;

    public BoardBlueprintLoader(IFigureCreator figureCreator)
    {
        _figureCreator = figureCreator;
    }
    
    public void LoadMap(IBoard board, BoardBlueprint map)
    {
        var index = 0;
        foreach (var tile in board)
        {
            tile.Figure = _figureCreator.CreateFigure(map.Figures[index++]);
        }
    }

    public void LoadMapExtendedFor2Players(IBoard board, BoardBlueprint map)
    {
        for (var i = 0; i < map.Figures.Length; i++)
        {
            var redFigure = map.Figures[i];
            var redPosition = Position.FromIndex(i + 64 - 16);
            
            var oppositeFigure = map.Figures[i].Player == 0
                ? redFigure
                : new FigureBlueprint(Player.Black, redFigure.FigureId, redFigure.IsKing);
            board[new Position(redPosition.X, 7 - redPosition.Y)].Figure = _figureCreator.CreateFigure(oppositeFigure);
            board[redPosition].Figure = _figureCreator.CreateFigure(redFigure);
        }
        
        for (var i = 16; i < 48; i++)
        {
            board[Position.FromIndex(i)].Figure = _figureCreator.CreateEmptyFigure();
        }
    }
}
