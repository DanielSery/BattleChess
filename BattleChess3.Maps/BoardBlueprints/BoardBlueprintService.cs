using System.Text.Json;
using BattleChess3.Core.GameBoard;
using BattleChess3.Maps.IO;
using BattleChess3.Maps.Utilities;

namespace BattleChess3.Maps.BoardBlueprints;

internal class BoardBlueprintService : IBoardBlueprintService
{
    private readonly IFileHandler _fileHandler;
    private readonly IDirectoryHandler _directoryHandler;

    public BoardBlueprintService(
        IFileHandler fileHandler,
        IDirectoryHandler directoryHandler)
    {
        _fileHandler = fileHandler;
        _directoryHandler = directoryHandler;
        CurrentMap = LoadMap();
    }

    public BoardBlueprint CurrentMap { get; private set; }

    public void Save(BoardBlueprint map)
    {
        CurrentMap = map;
        var text = JsonSerializer.Serialize(map);
        text = CompressionHelper.Compress(text);
        _fileHandler.WriteAllText("Resources\\TeamBoard.map", text);
    }

    private BoardBlueprint LoadMap()
    {
        const string directory = "Resources";
        const string filePath = "Resources\\TeamBoard.map";

        if (!_directoryHandler.Exists(directory))
        {
            _directoryHandler.CreateDirectory(directory);
            return BoardBlueprint.ChessTeam;
        }

        if (!_fileHandler.Exists(filePath))
        {
            return BoardBlueprint.ChessTeam;
        }

        try
        {
            var text = _fileHandler.ReadAllText(filePath);
            text = CompressionHelper.Decompress(text);

            var deserialized = JsonSerializer.Deserialize<BoardBlueprint>(text);
            if (deserialized is not null && IsBoardValid(deserialized))
                return deserialized;

            TryDeleteFile(filePath);
            return BoardBlueprint.ChessTeam;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            TryDeleteFile(filePath);
            return BoardBlueprint.ChessTeam;
        }
    }

    private static bool IsBoardValid(BoardBlueprint board)
    {
        return board.Figures.Length == 16 &&
               board.Figures.Count(x => x.IsKing) == 1;
    }

    private void TryDeleteFile(string filePath)
    {
        try
        {
            _fileHandler.Delete(filePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}