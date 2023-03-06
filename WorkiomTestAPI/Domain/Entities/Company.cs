using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkiomTestAPI.Domain.Entities
{
    public class Company
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonIgnore]
        public string Id { get { return ObjectId != null ? ObjectId.ToString() : ""; } }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("NumberOfEmployees")]
        public int NumberOfEmployees { get; set; }
    }
}