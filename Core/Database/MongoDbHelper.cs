using MongoDB.Driver;
using MongoDB.Bson;
using PlaywrightTestFramework.Core.Configuration;

namespace PlaywrightTestFramework.Core.Database;

public class MongoDbHelper
{
    private readonly IMongoDatabase _database;
    private readonly TestConfiguration _config;

    public MongoDbHelper(TestConfiguration config)
    {
        _config = config;
        var client = new MongoClient(_config.MongoConnectionString);
        _database = client.GetDatabase(_config.MongoDatabaseName);
    }

    public async Task<List<T>> GetCollectionAsync<T>(string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        return await collection.Find(_ => true).ToListAsync();
    }

    public async Task<T> GetDocumentByIdAsync<T>(string collectionName, string id)
    {
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task InsertDocumentAsync<T>(string collectionName, T document)
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.InsertOneAsync(document);
    }

    public async Task UpdateDocumentAsync<T>(string collectionName, string id, UpdateDefinition<T> update)
    {
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await collection.UpdateOneAsync(filter, update);
    }

    public async Task DeleteDocumentAsync<T>(string collectionName, string id)
    {
        var collection = _database.GetCollection<T>(collectionName);
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await collection.DeleteOneAsync(filter);
    }

    public async Task<long> GetDocumentCountAsync<T>(string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        return await collection.CountDocumentsAsync(_ => true);
    }

    public async Task CleanupCollectionAsync<T>(string collectionName)
    {
        var collection = _database.GetCollection<T>(collectionName);
        await collection.DeleteManyAsync(_ => true);
    }

    // Test data setup methods
    public async Task SeedTestDataAsync<T>(string collectionName, List<T> testData)
    {
        await CleanupCollectionAsync<T>(collectionName);
        var collection = _database.GetCollection<T>(collectionName);
        await collection.InsertManyAsync(testData);
    }
}
