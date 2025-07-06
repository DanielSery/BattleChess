using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BattleChess3.Multiplayer.Tables;

public class GameConfirm
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string GameId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string RequestId { get; set; }
}