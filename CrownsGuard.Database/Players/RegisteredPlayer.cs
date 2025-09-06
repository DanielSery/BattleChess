using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrownsGuard.Database.Players;

public class RegisteredPlayer
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public required string Name { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public required string PasswordSalt { get; set; }
    
    public required string EmailHash { get; set; }
    
    public short Elo { get; set; }
    
    public required byte[] UnlockedFigures { get; set; }
    
    public required byte[] Map { get; set; }
}