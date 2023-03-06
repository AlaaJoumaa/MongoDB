using MongoDB.Bson;

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
    }
}
