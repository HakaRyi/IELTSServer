using System.Collections.Generic;
using System.Threading.Tasks;
using KnowledgeBase.Domain.Entities;
using KnowledgeBase.Domain.Interfaces;
using KnowledgeBase.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KnowledgeBase.Infrastructure.Repositories;

public class GeneratedPassageRepository : IGeneratedPassageRepository
{
    private readonly IMongoCollection<GeneratedPassage> _collection;

    public GeneratedPassageRepository(IOptions<MongoDbSettings> settings, IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<GeneratedPassage>("GeneratedPassages");
    }

    public async Task<GeneratedPassage?> GetByIdAsync(string id) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<GeneratedPassage>> GetByTopicAsync(string topic) =>
        await _collection.Find(x => x.Topic == topic).ToListAsync();

    public async Task CreateAsync(GeneratedPassage passage) =>
        await _collection.InsertOneAsync(passage);
}