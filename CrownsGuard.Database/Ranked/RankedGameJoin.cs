using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrownsGuard.Database.Ranked;

public class RankedGameJoin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public required string GameId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PlayerId { get; set; }
    
    public required byte[] Map { get; set; }
}