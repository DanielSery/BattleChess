namespace BattleChess3.Multiplayer.Lobby;

public class PublicLobbyData
{
    public required string Id { get; set; }
    public required string LobbyName { get; set; }
    public required string Locked { get; set; }
    public string? JoinedId { get; set; }
    public short? Elo { get; set; }
}