using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using WorkiomTestAPI.Domain.DTOs;
using WorkiomTestAPI.Domain.Entities;
using WorkiomTestAPI.Domain.Extensions;
using WorkiomTestAPI.Filters;

namespace WorkiomTestAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<Company> _mongoCollection;
        private readonly IMongoCollection<BsonDocument> _bsonMongoCollection;

        public CompanyController(ILogger<CompanyController> logger, 
                                 MongoClient mongoClient)
        {
            _logger = logger;
            _mongoDatabase = mongoClient.GetDatabase("WorkiomTestDB");
            _mongoClient = new MongoClient("mongodb://localhost:27017");
            _mongoCollection = _mongoDatabase.GetCollection<Company>(nameof(Company));
            _bsonMongoCollection = _mongoDatabase.GetCollection<BsonDocument>(nameof(Company));
        }

        [HttpPost, Route("api/company/create")]
        public async Task<IActionResult> Create([FromBody]Company company)
        {
            try
            {
                await _mongoCollection.InsertOneAsync(company);
                return new JsonResult(new 
                                     {
                                        Id = company.Id.ToString()
                                     });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("api/company/get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var filter = Builders<Company>.Filter.Eq("_id", new ObjectId(id));
                var result = await _mongoCollection.Find(filter).FirstOrDefaultAsync();

                return new JsonResult(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet,Route("api/company/list")]
        public async Task<IActionResult> List([FromBody]CompanyFilter companyFilter)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Empty;
                if (!string.IsNullOrWhiteSpace(companyFilter.Id))
                    filter &= Builders<BsonDocument>.Filter.Eq("_id", companyFilter.Id);
                if (!string.IsNullOrWhiteSpace(companyFilter.Name))
                    filter &= Builders<BsonDocument>.Filter.Eq(nameof(Company.Name), companyFilter.Name);
                if (companyFilter.NumberOfEmployees > 0)
                    filter &= Builders<BsonDocument>.Filter.Eq(nameof(Company.NumberOfEmployees), companyFilter.NumberOfEmployees);

                if (companyFilter.ExtendColumns != null && companyFilter.ExtendColumns.Count > 0)
                {
                    foreach (var extendColumn in companyFilter.ExtendColumns)
                    {
                        filter &= Builders<BsonDocument>.Filter.EqType(extendColumn.Name, extendColumn.Value, extendColumn.Type);
                    }
                }
                var result = (await (await _bsonMongoCollection.FindAsync(filter)).ToListAsync())
                                                               .Select(x => BsonSerializer.Deserialize<dynamic>(x))
                                                               .ToList();

                return new JsonResult(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route("api/company/update")]
        public async Task<IActionResult> Update([FromQuery]string id, [FromBody]Company company)
        {
            try
            {
                company.ObjectId = new ObjectId(id);
                var filter = Builders<Company>.Filter.Eq("_id", new ObjectId(id));
                var replaceOneResult  = await _mongoCollection.ReplaceOneAsync(filter, company);
             
                return new JsonResult(new
                {
                    Updated = replaceOneResult.ModifiedCount > 0
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete, Route("api/company/delete")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            try
            {
                var filter = Builders<Company>.Filter.Eq("_id", new ObjectId(id));
                var deleteResult = await _mongoCollection.DeleteOneAsync(filter);
                return new JsonResult(new
                {
                    Deleted = deleteResult.DeletedCount > 0
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost, Route("api/company/extend")]
        public async Task<IActionResult> Extend([FromQuery] string id, List<ExtendColumn> extendColumns)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
                var result = (await (await _bsonMongoCollection.FindAsync(filter)).FirstOrDefaultAsync()).ToBsonDocument();
                foreach (var extendColumn in extendColumns)
                {
                    var doc = result.ToBsonDocument();
                    if (!doc.Contains(extendColumn.Name)!)
                        result = result.ToBsonDocument().Add(extendColumn.Name, extendColumn.Value, extendColumn.Type);
                }
                var replaceOneResult = await _bsonMongoCollection.ReplaceOneAsync(filter, result);

                return new JsonResult(new
                {
                    Extended = replaceOneResult.ModifiedCount > 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}