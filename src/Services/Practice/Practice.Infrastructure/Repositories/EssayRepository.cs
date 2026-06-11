using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Practice.Application.Interfaces;
using Practice.Domain.Entities;
using Practice.Infrastructure.Settings;

namespace Practice.Infrastructure.Repositories;

public class EssayRepository : IEssayRepository
{
    private readonly IMongoCollection<GeneratedEssay> _collection;

    public EssayRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var db = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = db.GetCollection<GeneratedEssay>(settings.Value.EssaysCollectionName);
    }

    public async Task<GeneratedEssay> CreateAsync(GeneratedEssay essay)
    {
        await _collection.InsertOneAsync(essay);
        return essay;
    }

    public async Task<GeneratedEssay?> GetByIdAsync(string userId, string id)
        => await _collection.Find(e => e.Id == id && e.UserId == userId).FirstOrDefaultAsync();

    public async Task<List<GeneratedEssay>> GetRecentAsync(string userId, int limit)
        => await _collection.Find(e => e.UserId == userId)
            .SortByDescending(e => e.CreatedAt)
            .Limit(limit)
            .ToListAsync();
}
