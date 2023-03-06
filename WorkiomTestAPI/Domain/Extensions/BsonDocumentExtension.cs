using MongoDB.Bson;
using MongoDB.Driver;
using System.Xml.Linq;

namespace WorkiomTestAPI.Domain.Extensions
{
    public static class BsonDocumentExtension
    {
        public static BsonDocument Add(this BsonDocument doc, string name, object value, BsonType type)
        {
            switch (type)
            {
                case BsonType.String:
                    doc.Add(name, value as string);
                    break;
                case BsonType.Int32:
                    doc.Add(name, int.Parse(value.ToString()!));
                    break;
                case BsonType.Int64:
                    doc.Add(name, Int64.Parse(value.ToString()!));
                    break;
                case BsonType.DateTime:
                    doc.Add(name, DateTime.Parse(value.ToString()!));
                    break;
                case BsonType.Double:
                    doc.Add(name, double.Parse(value.ToString()!));
                    break;
                case BsonType.Decimal128:
                    doc.Add(name, decimal.Parse(value.ToString()!));
                    break;

            }
            return doc;
        }

        public static FilterDefinition<TDocument> EqType<TDocument>(this FilterDefinitionBuilder<TDocument> builderFilter, string name, object value, BsonType type)
        {
            var filter = Builders<TDocument>.Filter.Empty;
            switch (type)
            {
                case BsonType.String:
                    filter = builderFilter.Eq(name, value as string);
                    break;
                case BsonType.Int32:
                    filter = builderFilter.Eq(name, int.Parse(value.ToString()!));
                    break;
                case BsonType.Int64:
                    filter = builderFilter.Eq(name, Int64.Parse(value.ToString()!));
                    break;
                case BsonType.DateTime:
                    filter = builderFilter.Eq(name, DateTime.Parse(value.ToString()!));
                    break;
                case BsonType.Double:
                    filter = builderFilter.Eq(name, double.Parse(value.ToString()!));
                    break;
                case BsonType.Decimal128:
                    filter = builderFilter.Eq(name, decimal.Parse(value.ToString()!));
                    break;
            }
            return filter;
        }
    }
}
