using BattleChess3.Core;
using BattleChess3.Core.Figures;
using BattleChess3.Core.GameBoard;
using BattleChess3.Core.Players;
using BattleChess3.Maps.Figures;

namespace BattleChess3.Maps.BoardBlueprints;

internal class BoardLoader : IBoardLoader
{
    private readonly IFigureCreator _figureCreator;

    public BoardLoader(IFigureCreator figureCreator)
    {
        _figureCreator = figureCreator;
    }
    
    public void LoadBoard(IBoard board, BoardBlueprint map)
    {
        if (board.Count() != Constants.FullBoardTilesCount) throw new ArgumentException("Full board needs to have 64 tiles");
        if (map.Figures.Length != Constants.FullBoardTilesCount) throw new ArgumentException("Map blueprint needs to have 64 tiles");
        if (map.Figures.Count(x => x is { IsKing: true, Player: Player.White }) != 1) throw new ArgumentException("Map blueprint needs to have a white king");
        if (map.Figures.Count(x => x is { IsKing: true, Player: Player.Black }) != 1) throw new ArgumentException("Map blueprint needs to have a black king");

        var index = 0;
        foreach (var tile in board)
        {
            tile.Figure = _figureCreator.CreateFigure(map.Figures[index++]);
        }
    }

    public void LoadTeamBoard(IBoard board, BoardBlueprint map)
    {
        if (board.Count() != map.Figures.Length) throw new ArgumentException("Source and target map size must match");
        if (map.Figures.Count(x => x is { IsKing: true, Player: Player.White }) != 1) throw new ArgumentException("Map blueprint needs to have a white king");
        if (map.Figures.Any(x => x is { Player: Player.Black })) throw new ArgumentException("Partial map blueprint cannot have black figure");

        var index = 0;
        foreach (var tile in board)
        {
            tile.Figure = _figureCreator.CreateFigure(map.Figures[index++]);
        }
    }

    public void LoadBoardExtendedFor2Players(IBoard board, BoardBlueprint map)
    {
        if (board.Count() != Constants.FullBoardTilesCount) throw new ArgumentException("Full board needs to have 64 tiles");
        if (map.Figures.Length != 16) throw new ArgumentException("Partial map blueprint needs to have 16 tiles");
        if (map.Figures.Count(x => x is { IsKing: true, Player: Player.White }) != 1) throw new ArgumentException("Map blueprint needs to have a white king");
        if (map.Figures.Any(x => x is { Player: Player.Black })) throw new ArgumentException("Partial map blueprint cannot have black figure");

        for (var i = 0; i < map.Figures.Length; i++)
        {
            var whiteFigure = map.Figures[i];
            var whitePosition = Position.FromIndex(i + 64 - 16);
            board[whitePosition].Figure = _figureCreator.CreateFigure(whiteFigure);
            
            var blackFigure = new FigureBlueprint(Player.Black, whiteFigure.FigureId, whiteFigure.IsKing);
            var blackPosition = new Position(whitePosition.X, 7 - whitePosition.Y);
            board[blackPosition].Figure = _figureCreator.CreateFigure(blackFigure);
        }
        
        for (var i = 16; i < 48; i++)
        {
            board[Position.FromIndex(i)].Figure = _figureCreator.CreateEmptyFigure();
        }
    }
}
