namespace BattleChess3.Multiplayer.Tables;

public class PublicLobbyData
{
    public string LobbyName { get; set; }
    public string Locked { get; set; }
    public short? Elo { get; set; }
}