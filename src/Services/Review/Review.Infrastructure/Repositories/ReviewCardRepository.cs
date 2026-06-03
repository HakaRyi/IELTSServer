using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Review.Application.Interfaces;
using Review.Domain.Entities;
using Review.Infrastructure.Settings;

namespace Review.Infrastructure.Repositories;

public class ReviewCardRepository : IReviewCardRepository
{
    private readonly IMongoCollection<ReviewCard> _col;

    public ReviewCardRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> settings)
    {
        var db = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _col = db.GetCollection<ReviewCard>(settings.Value.ReviewCardsCollectionName);
    }

    public async Task<ReviewCard> CreateAsync(ReviewCard card)
    {
        await _col.InsertOneAsync(card);
        return card;
    }

    public Task<ReviewCard?> GetByIdAsync(string id)
        => _col.Find(c => c.Id == id).FirstOrDefaultAsync()!;

    public Task<ReviewCard?> GetByLexicalItemIdAsync(string lexicalItemId)
        => _col.Find(c => c.LexicalItemId == lexicalItemId).FirstOrDefaultAsync()!;

    public Task UpdateAsync(ReviewCard card)
        => _col.ReplaceOneAsync(c => c.Id == card.Id, card);

    public Task DeleteAsync(string id)
        => _col.DeleteOneAsync(c => c.Id == id);

    public async Task<List<ReviewCard>> GetDueAsync(DateTime asOf)
        => await _col.Find(c => c.NextReviewAt <= asOf)
            .SortBy(c => c.NextReviewAt)
            .ToListAsync();

    public async Task<List<ReviewCard>> GetAllAsync()
        => await _col.Find(_ => true)
            .SortByDescending(c => c.EnrolledAt)
            .ToListAsync();

    public async Task<(int Total, int Due, int Mastered)> GetStatsAsync(DateTime asOf)
    {
        var all = await _col.Find(_ => true).ToListAsync();
        int total = all.Count;
        int due = all.Count(c => c.NextReviewAt <= asOf);
        int mastered = all.Count(c => c.Interval >= 21);
        return (total, due, mastered);
    }
}
