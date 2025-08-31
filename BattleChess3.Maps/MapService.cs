using System.Text.Json;
using BattleChess3.Core.GameBoard;
using BattleChess3.Maps.Utilities;

namespace BattleChess3.Maps;

internal class MapService : IMapService
{
    private readonly TaskCompletionSource _taskCompletionSource = new TaskCompletionSource();
    private BoardBlueprint _map = BoardBlueprint.ChessTeam;

    public MapService()
    {
        Task.Run(LoadMap);
    }

    public BoardBlueprint GetCurrentMap()
    {
        _taskCompletionSource.Task.Wait();
        return _map;
    }

    public void Save(BoardBlueprint map)
    {
        var text = JsonSerializer.Serialize(map);
        text = CompressionHelper.Compress(text);
        File.WriteAllText("Resources/TeamBoard.map", text);
    }

    private void LoadMap()
    {
        if (!Directory.Exists("Resources"))
        {
            Directory.CreateDirectory("Resources");
            _map = BoardBlueprint.ChessTeam;
            _taskCompletionSource.SetResult();
            return;
        }
        
        _map = Directory.GetFiles("Resources", "TeamBoard.map")
            .Where(path => File.Exists(Path.GetFullPath(path)))
            .Select(path =>
            {
                var text = File.ReadAllText(Path.GetFullPath(path));
                text = CompressionHelper.Decompress(text);
                return JsonSerializer.Deserialize<BoardBlueprint>(text);
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .FirstOrDefault() ?? BoardBlueprint.ChessTeam;

        _taskCompletionSource.TrySetResult();
    }
}