namespace BattleChess3.Multiplayer.Tables;

public class PublicLobbyData
{
    public string LobbyName { get; set; }
    public short Elo { get; set; }
    public bool IsHostStarting { get; set; }
    public string PlayerName { get; set; }
    public bool IsLocked { get; set; }
}