using KnowledgeBase.Domain.Entities;
using KnowledgeBase.Domain.Interfaces;
using KnowledgeBase.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace KnowledgeBase.Infrastructure.Repositories;

public class LexicalItemRepository : ILexicalItemRepository
{
    private readonly IMongoCollection<LexicalItem> _collection;

    public LexicalItemRepository(IOptions<MongoDbSettings> settings, IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<LexicalItem>(settings.Value.LexicalItemsCollectionName);
    }

    public async Task<List<LexicalItem>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<LexicalItem?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(LexicalItem item) =>
        await _collection.InsertOneAsync(item);

    public async Task UpdateAsync(string id, LexicalItem item) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, item);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id);

    public async Task<LexicalItem?> GetByValueAsync(string value)
    {
        var filter = Builders<LexicalItem>.Filter.Regex(
            x => x.Value,
            new BsonRegularExpression($"^{Regex.Escape(value)}$", "i"));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<(List<LexicalItem> Items, long Total)> GetPagedAsync(string? topic, int page, int pageSize)
    {
        var filter = string.IsNullOrWhiteSpace(topic)
            ? Builders<LexicalItem>.Filter.Empty
            : Builders<LexicalItem>.Filter.AnyEq(x => x.Topics, topic);

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<string>> GetDistinctTopicsAsync()
    {
        var topics = await _collection.Distinct<string>("Topics", FilterDefinition<LexicalItem>.Empty).ToListAsync();
        return topics.OrderBy(t => t).ToList();
    }
}