using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;
using Practice.Infrastructure.Settings;

namespace Practice.Infrastructure.Repositories;

public class GeneratedPassageRepository : IGeneratedPassageRepository
{
    private readonly IMongoCollection<GeneratedPassage> _collection;

    public GeneratedPassageRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var db = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = db.GetCollection<GeneratedPassage>(settings.Value.PassagesCollectionName);
    }

    public async Task<GeneratedPassage> CreateAsync(GeneratedPassage passage)
    {
        await _collection.InsertOneAsync(passage);
        return passage;
    }

    public async Task<List<GeneratedPassage>> GetByTopicAsync(string topic)
    {
        return await _collection
            .Find(p => p.Topic == topic)
            .SortByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<GeneratedPassage?> GetByIdAsync(string id)
        => await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task<List<GeneratedPassage>> GetRecentAsync(int limit)
        => await _collection.Find(_ => true)
            .SortByDescending(p => p.CreatedAt)
            .Limit(limit)
            .ToListAsync();
}
