using MongoDB.Bson.Serialization.Attributes;

namespace WorkiomTestAPI.Domain.DTOs
{
    public class RefId
    {
        [BsonElement("Id")]
        public string Id { get; set; }
    }
}
