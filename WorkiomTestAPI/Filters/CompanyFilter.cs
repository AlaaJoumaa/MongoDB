using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using WorkiomTestAPI.Domain.DTOs;

namespace WorkiomTestAPI.Filters
{
    public class CompanyFilter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int? NumberOfEmployees { get; set; }
        public ICollection<ExtendColumn> ExtendColumns { get; set; }
    }
}
