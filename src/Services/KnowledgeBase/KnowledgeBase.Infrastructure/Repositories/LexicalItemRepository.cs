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

    public async Task<List<LexicalItem>> GetAllAsync(string userId) =>
        await _collection.Find(x => x.UserId == userId).ToListAsync();

    public async Task<LexicalItem?> GetByIdAsync(string userId, string id) =>
        await _collection.Find(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();

    public async Task CreateAsync(LexicalItem item) =>
        await _collection.InsertOneAsync(item);

    public async Task UpdateAsync(string id, LexicalItem item) =>
        await _collection.ReplaceOneAsync(x => x.Id == id, item);

    public async Task DeleteAsync(string userId, string id) =>
        await _collection.DeleteOneAsync(x => x.Id == id && x.UserId == userId);

    public async Task<LexicalItem?> GetByValueAsync(string userId, string value)
    {
        var filter = Builders<LexicalItem>.Filter.And(
            Builders<LexicalItem>.Filter.Eq(x => x.UserId, userId),
            Builders<LexicalItem>.Filter.Regex(
                x => x.Value,
                new BsonRegularExpression($"^{Regex.Escape(value)}$", "i")));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<(List<LexicalItem> Items, long Total)> GetPagedAsync(
        string userId, string? topic, int page, int pageSize)
    {
        var byUser = Builders<LexicalItem>.Filter.Eq(x => x.UserId, userId);
        var filter = string.IsNullOrWhiteSpace(topic)
            ? byUser
            : Builders<LexicalItem>.Filter.And(
                byUser,
                Builders<LexicalItem>.Filter.AnyEq(x => x.Topics, topic));

        var total = await _collection.CountDocumentsAsync(filter);
        var items = await _collection.Find(filter)
            .SortByDescending(x => x.CreatedAt)   // mới tra lên trước (nhóm theo ngày)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<List<string>> GetDistinctTopicsAsync(string userId)
    {
        var filter = Builders<LexicalItem>.Filter.Eq(x => x.UserId, userId);
        var topics = await _collection.Distinct<string>("Topics", filter).ToListAsync();
        return topics.OrderBy(t => t).ToList();
    }

    public async Task<List<(string Topic, long Count)>> GetTopicStatsAsync(string userId)
    {
        // unwind Topics → group đếm số từ mỗi chủ đề
        var pipeline = new[]
        {
            new BsonDocument("$match", new BsonDocument("UserId", userId)),
            new BsonDocument("$unwind", "$Topics"),
            new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$Topics" },
                { "count", new BsonDocument("$sum", 1) }
            }),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };
        var docs = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
        return docs
            .Select(d => (d["_id"].AsString, d["count"].ToInt64()))
            .ToList();
    }

    public async Task<List<LexicalItem>> SearchByPrefixAsync(string userId, string prefix, int limit)
    {
        var filter = Builders<LexicalItem>.Filter.And(
            Builders<LexicalItem>.Filter.Eq(x => x.UserId, userId),
            Builders<LexicalItem>.Filter.Regex(
                x => x.Value,
                new BsonRegularExpression($"^{Regex.Escape(prefix)}", "i")));
        return await _collection.Find(filter)
            .SortBy(x => x.Value)
            .Limit(limit)
            .ToListAsync();
    }
}
