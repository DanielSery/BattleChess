using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CrownsGuard.Database.Ranked;

public class RankedGame
{
    [BsonId] // Automatically maps to MongoDB's _id field
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PlayerId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? JoinedId { get; set; }
    
    public int Version { get; set; }
    public short Elo { get; set; }
    public bool IsHostStarting { get; set; }

    public required byte[] Map { get; set; }
}