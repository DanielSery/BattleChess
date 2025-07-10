using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BattleChess3.Multiplayer.Tables;

public class RankedGameJoin
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string GameId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string? PlayerId { get; set; }
    
    public byte[] Map { get; set; }
}