using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BattleChess3.Multiplayer.Tables;

public class GameRequest
{
    [BsonId] // Automatically maps to MongoDB's _id field
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    public bool IsPublic { get; set; }
    
    public int Version { get; set; }
    public short Elo { get; set; }
    public bool IsHostStarting { get; set; }

    public byte[] Map { get; set; }
}