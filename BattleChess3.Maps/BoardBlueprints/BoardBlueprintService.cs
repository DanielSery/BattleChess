using System.Text.Json;
using BattleChess3.Core.GameBoard;
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
        _fileHandler.WriteAllText("Resources/TeamBoard.map", text);
    }

    private BoardBlueprint LoadMap()
    {
        if (!_directoryHandler.Exists("Resources"))
        {
            _directoryHandler.CreateDirectory("Resources");
            return BoardBlueprint.ChessTeam;
        }

        return _directoryHandler.GetFiles("Resources", "TeamBoard.map")
            .Where(path => _fileHandler.Exists(Path.GetFullPath(path)))
            .Select(path =>
            {
                var text = _fileHandler.ReadAllText(Path.GetFullPath(path));
                text = CompressionHelper.Decompress(text);
                return JsonSerializer.Deserialize<BoardBlueprint>(text);
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .FirstOrDefault() ?? BoardBlueprint.ChessTeam;
    }
}