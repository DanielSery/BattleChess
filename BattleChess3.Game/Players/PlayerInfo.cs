using System.Diagnostics;
using BattleChess3.Game.Figures;

namespace BattleChess3.Game.Players;

[DebuggerDisplay("{Name}")]
public class PlayerInfo : IPlayerInfo
{
    public static readonly PlayerInfo Neutral = new(Player.Neutral, "Neutral", InfinitePlayerTimer.Instance);

    public PlayerInfo(Player player, string playerName, IPlayerTimer timer)
    {
        Player = player;
        Name = playerName;
        Timer = timer;
    }

    public Player Player { get; }
    public IPlayerTimer Timer { get; }
    public string Name { get; }
    
    public List<Figure> Figures { get; } = [];
}