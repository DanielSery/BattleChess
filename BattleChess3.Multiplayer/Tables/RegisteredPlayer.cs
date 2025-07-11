using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BattleChess3.Multiplayer.Tables;

public class RegisteredPlayer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public string Name { get; set; }
    
    public string PasswordHash { get; set; }
    
    public string PasswordSalt { get; set; }
    
    public string EmailHash { get; set; }
    
    public short Elo { get; set; }
    
    public byte[] UnlockedFigures { get; set; }
}