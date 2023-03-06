using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WorkiomTestAPI.Domain.DTOs;
using WorkiomTestAPI.Domain.Entities;

namespace WorkiomTestAPI.Filters
{
    public class ContactFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<RefId> Companies { get; set; }
        public ICollection<ExtendColumn> ExtendColumns { get; set; }
    }
}
