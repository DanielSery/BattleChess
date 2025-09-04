namespace BattleChess3.Multiplayer.Players;

public class PublicPlayerData
{
    public int Rank { get; set; }
    public required string Name { get; set; }
    public short Elo { get; set; }
}