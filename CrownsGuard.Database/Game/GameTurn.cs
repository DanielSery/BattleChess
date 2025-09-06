using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrownsGuard.Database.Game;

public class GameTurn
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public required string GameId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public double TimeSpentInSeconds { get; set; }
    
    public byte FromIndex { get; set; }
    public byte ToIndex { get; set; }
}