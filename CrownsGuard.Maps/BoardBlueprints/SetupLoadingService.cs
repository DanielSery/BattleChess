using System.Text.Json;
using CrownsGuard.Core.Board;
using CrownsGuard.Core.Figures;
using CrownsGuard.Core.Helpers;
using CrownsGuard.Maps.IO;
using CrownsGuard.Maps.Utilities;

namespace CrownsGuard.Maps.BoardBlueprints;

internal class SetupLoadingService : ISetupLoadingService
{
    private readonly IFileHandler _fileHandler;
    private readonly IDirectoryHandler _directoryHandler;

    public SetupLoadingService(
        IFileHandler fileHandler,
        IDirectoryHandler directoryHandler)
    {
        _fileHandler = fileHandler;
        _directoryHandler = directoryHandler;
        CurrentMap = LoadMap();
    }

    public Figure[] CurrentMap { get; private set; }

    public void Save(Figure[] map)
    {
        CurrentMap = map;
        var text = JsonSerializer.Serialize(map);
        text = CompressionHelper.Compress(text);
        _fileHandler.WriteAllText("Resources\\TeamBoard.map", text);
    }

    private Figure[] LoadMap()
    {
        const string directory = "Resources";
        const string filePath = "Resources\\TeamBoard.map";

        if (!_directoryHandler.Exists(directory))
        {
            _directoryHandler.CreateDirectory(directory);
            return SampleSetup.ChessSetup;
        }

        if (!_fileHandler.Exists(filePath))
        {
            return SampleSetup.ChessSetup;
        }

        try
        {
            var text = _fileHandler.ReadAllText(filePath);
            text = CompressionHelper.Decompress(text);

            var deserialized = JsonSerializer.Deserialize<Figure[]>(text);
            if (deserialized is not null && IsBoardValid(deserialized))
                return deserialized;

            TryDeleteFile(filePath);
            return SampleSetup.ChessSetup;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            TryDeleteFile(filePath);
            return SampleSetup.ChessSetup;
        }
    }

    private static bool IsBoardValid(Figure[] board)
    {
        return board.Length == 16 &&
               board.Count(x => x.IsKing()) == 1;
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