using BattleChess3.Core.Model;

namespace BattleChess3.UI.Services;

public interface IMultiplayerService
{
    public bool IsHost { get; }
    public bool IsGuest { get; }
    public event EventHandler<Position>? RequestClickTile;
    public event EventHandler<MapBlueprint>? RequestLoadMap;
    public void Host(string apiKey, MapBlueprint map, Position selectedPosition);
    public void Join(string apiKey);
    public void Stop();

    public void LoadMap(MapBlueprint map);
    public void ClickOnPosition(Position position);
}