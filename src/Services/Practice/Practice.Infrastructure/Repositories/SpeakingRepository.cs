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

    public async Task<List<GeneratedSpeaking>> GetByTopicAsync(string userId, string topic)
        => await _collection
            .Find(s => s.UserId == userId && s.Topic == topic)
            .SortByDescending(s => s.CreatedAt)
            .ToListAsync();

    public async Task<GeneratedSpeaking?> GetByIdAsync(string userId, string id)
        => await _collection.Find(s => s.Id == id && s.UserId == userId).FirstOrDefaultAsync();

    public async Task<List<GeneratedSpeaking>> GetRecentAsync(string userId, int limit)
        => await _collection.Find(s => s.UserId == userId)
            .SortByDescending(s => s.CreatedAt)
            .Limit(limit)
            .ToListAsync();
}
