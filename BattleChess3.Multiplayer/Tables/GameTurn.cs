using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BattleChess3.Multiplayer.Tables;

public class GameTurn
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string GameId { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public double TimeSpentInSeconds { get; set; }
    
    public byte FromIndex { get; set; }
    public byte ToIndex { get; set; }
}