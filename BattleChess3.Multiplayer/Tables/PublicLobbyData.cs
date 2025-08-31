namespace BattleChess3.Multiplayer.Tables;

public class PublicLobbyData
{
    public string Id { get; set; }
    public string LobbyName { get; set; }
    public string Locked { get; set; }
    public string? JoinedId { get; set; }
    public short? Elo { get; set; }
}