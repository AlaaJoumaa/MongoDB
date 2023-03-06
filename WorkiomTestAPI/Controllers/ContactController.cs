using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Nodes;
using WorkiomTestAPI.Domain.DTOs;
using WorkiomTestAPI.Domain.Entities;
using WorkiomTestAPI.Domain.Extensions;
using WorkiomTestAPI.Filters;

namespace WorkiomTestAPI.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IMongoCollection<Contact> _mongoCollection;
        private readonly IMongoCollection<BsonDocument> _bsonMongoCollection;

        public ContactController(ILogger<ContactController> logger, 
                                 MongoClient mongoClient)
        {
            _logger = logger;
            _mongoDatabase = mongoClient.GetDatabase("WorkiomTestDB");
            _mongoClient = new MongoClient("mongodb://localhost:27017");
            _mongoCollection = _mongoDatabase.GetCollection<Contact>(nameof(Contact));
            _bsonMongoCollection = _mongoDatabase.GetCollection<BsonDocument>(nameof(Contact));
        }

        [HttpPost, Route("api/contact/create")]
        public async Task<IActionResult> Create([FromBody] Contact contact)
        {
            try
            {
                //var lst = new List<Contact>();
                //for(var i = 0; i < 2000000; i++)
                //{
                //    if (i <= 1000000)
                //        lst.Add(new Contact() { Name = $"Cont {i}", Companies = new List<RefId>() { new RefId() { Id = "640387adbf51ad735b3cfe7d" } } });
                //    else
                //        lst.Add(new Contact() { Name = $"Cont {i}", Companies = new List<RefId>() { new RefId() { Id = "64039324ec363efa5f271932" } } });
                //}

                //await _mongoCollection.InsertManyAsync(lst);
                await _mongoCollection.InsertOneAsync(contact);
                return new JsonResult(new 
                                     {
                                        Id = contact.Id.ToString()
                                     });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet, Route("api/contact/get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                var filter = Builders<Contact>.Filter.Eq("_id", new ObjectId(id));
                var result = await _mongoCollection.Find(filter).FirstOrDefaultAsync();

                return new JsonResult(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet,Route("api/contact/list")]
        public async Task<IActionResult> List([FromBody]ContactFilter contactFilter)
        {
            try
            {
                var filter = Builders<Contact>.Filter.Empty;
                if (!string.IsNullOrWhiteSpace(contactFilter.Id))
                    filter &= Builders<Contact>.Filter.Eq("_id", contactFilter.Id);
                if (!string.IsNullOrWhiteSpace(contactFilter.Name))
                    filter &= Builders<Contact>.Filter.Eq(x => x.Name, contactFilter.Name);
                if (contactFilter.Companies != null && contactFilter.Companies.Count > 0)
                    filter &= Builders<Contact>.Filter.AnyIn(x => x.Companies, contactFilter.Companies);

                var result = await (await _mongoCollection.FindAsync(filter)).ToListAsync();

                return new JsonResult(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut, Route("api/contact/update")]
        public async Task<IActionResult> Update([FromQuery]string id, [FromBody] Contact contact)
        {
            try
            {
                contact.ObjectId = new ObjectId(id);
                var filter = Builders<Contact>.Filter.Eq("_id", new ObjectId(id));
                var replaceOneResult  = await _mongoCollection.ReplaceOneAsync(filter, contact);
             
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

        [HttpDelete, Route("api/contact/delete")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            try
            {
                var filter = Builders<Contact>.Filter.Eq("_id", new ObjectId(id));
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

        [HttpPost, Route("api/contact/extend")]
        public async Task<IActionResult> Extend([FromQuery] string id, List<ExtendColumn> extendColumns)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(id));
                var result = (await (await _bsonMongoCollection.FindAsync(filter)).FirstOrDefaultAsync()).ToBsonDocument();
                foreach (var extendColumn in extendColumns)
                {
                    var doc = result.ToBsonDocument();
                    if(!doc.Contains(extendColumn.Name)!)
                        result = result.ToBsonDocument().Add(extendColumn.Name, extendColumn.Value, extendColumn.Type);
                }
                var replaceOneResult = await _bsonMongoCollection.ReplaceOneAsync(filter, result);

                return new JsonResult(new
                {
                    Extended = replaceOneResult.ModifiedCount > 0
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}