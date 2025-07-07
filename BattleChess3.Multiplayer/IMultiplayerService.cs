using BattleChess3.Game.Board;
using BattleChess3.Maps;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerService
{
    public bool IsHost { get; }
    public bool IsGuest { get; }
    
    public event EventHandler<(Position, Position)>? RequestPlayMove;
    public event EventHandler<MapBlueprint>? RequestLoadMap;

    public Task<string> Host(bool isPublic, bool isHostStarting, MapBlueprint myMap);
    public Task WaitForHostConfirmation(bool isHostStarting, MapBlueprint myMap);
    public void Join(string gameId, MapBlueprint myMap);
    public void Stop();
    public void PlayedMove(Position from, Position to);
}