using MongoDB.Bson;

namespace WorkiomTestAPI.Domain.DTOs
{
    public class ExtendColumn
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public BsonType Type { get; set; }
    }
}
