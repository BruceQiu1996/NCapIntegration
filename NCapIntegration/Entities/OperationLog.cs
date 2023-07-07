using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NCapIntegration.Entities
{
    public class OperateLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Author { get; set; }
        public string Topic { get; set; }
        public string Module { get; set; }
        public string Result { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? FinishTime { get; set; }
    }
}
