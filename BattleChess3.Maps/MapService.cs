using BattleChess3.Maps.Utilities;
using Newtonsoft.Json;

namespace BattleChess3.Maps;

internal class MapService : IMapService
{
    private readonly TaskCompletionSource _taskCompletionSource = new TaskCompletionSource();
    private MapBlueprint _map = MapBlueprint.EmptyTeam;

    public MapService()
    {
        Task.Run(LoadMap);
    }

    public MapBlueprint GetCurrentMap()
    {
        _taskCompletionSource.Task.Wait();
        return _map;
    }

    public void Save(MapBlueprint map)
    {
        var text = JsonConvert.SerializeObject(map);
        text = CompressionHelper.Compress(text);
        File.WriteAllText("Resources/TeamBoard.map", text);
    }

    private void LoadMap()
    {
        _map = Directory.GetFiles("Resources", "TeamBoard.map")
            .Where(path => File.Exists(Path.GetFullPath(path)))
            .Select(path =>
            {
                var text = File.ReadAllText(Path.GetFullPath(path));
                text = CompressionHelper.Decompress(text);
                return JsonConvert.DeserializeObject<MapBlueprint>(text);
            })
            .Where(x => x is not null)
            .Select(x => x!)
            .FirstOrDefault() ?? MapBlueprint.EmptyTeam;

        _taskCompletionSource.TrySetResult();
    }
}