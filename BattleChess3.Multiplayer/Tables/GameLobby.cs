using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BattleChess3.Multiplayer.Tables;

public class GameLobby
{
    [BsonId] // Automatically maps to MongoDB's _id field
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonRepresentation(BsonType.ObjectId)]
    public string? PlayerId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? JoinedId { get; set; }
    
    public string LobbyName { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    
    public int Version { get; set; }
    public short? Elo { get; set; }
    public bool IsHostStarting { get; set; }
    
    public byte[] Map { get; set; }
}