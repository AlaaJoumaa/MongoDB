using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WorkiomTestAPI.Domain.DTOs;

namespace WorkiomTestAPI.Domain.Entities
{
    public class Contact
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonIgnore]
        public string Id { get { return ObjectId != null ? ObjectId.ToString() : ""; } }
        [BsonElement("Name")]
        public string Name { get; set; }
        [BsonElement("Companies")]
        public ICollection<RefId> Companies { get; set; }
    }
}
