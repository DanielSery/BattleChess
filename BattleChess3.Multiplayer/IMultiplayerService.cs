using BattleChess3.Game.Board;
using BattleChess3.Maps;

namespace BattleChess3.Multiplayer;

public interface IMultiplayerService
{
    public bool IsHost { get; }
    public bool IsGuest { get; }
    public event EventHandler<(Position, Position)>? RequestPlayMove;
    public event EventHandler<MapBlueprint>? RequestLoadMap;
    public event EventHandler<string>? RequestDisplayMessage; 
    
    public void Host(uint gameId, MapBlueprint map);
    public void Join(uint? gameId);
    public void Stop();
    public void PlayedMove(Position from, Position to);
}