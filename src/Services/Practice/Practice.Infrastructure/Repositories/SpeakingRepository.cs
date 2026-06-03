using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;
using Practice.Infrastructure.Settings;

namespace Practice.Infrastructure.Repositories;

public class SpeakingRepository : ISpeakingRepository
{
    private readonly IMongoCollection<GeneratedSpeaking> _collection;

    public SpeakingRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var db = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = db.GetCollection<GeneratedSpeaking>(settings.Value.SpeakingCollectionName);
    }

    public async Task<GeneratedSpeaking> CreateAsync(GeneratedSpeaking speaking)
    {
        await _collection.InsertOneAsync(speaking);
        return speaking;
    }

    public async Task<List<GeneratedSpeaking>> GetByTopicAsync(string topic)
    {
        return await _collection
            .Find(s => s.Topic == topic)
            .SortByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<GeneratedSpeaking?> GetByIdAsync(string id)
        => await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();

    public async Task<List<GeneratedSpeaking>> GetRecentAsync(int limit)
        => await _collection.Find(_ => true)
            .SortByDescending(s => s.CreatedAt)
            .Limit(limit)
            .ToListAsync();
}
