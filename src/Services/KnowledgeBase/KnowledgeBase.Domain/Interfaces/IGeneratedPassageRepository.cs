using System.Collections.Generic;
using System.Threading.Tasks;
using KnowledgeBase.Domain.Entities;

namespace KnowledgeBase.Domain.Interfaces;

public interface IGeneratedPassageRepository
{
    Task<GeneratedPassage?> GetByIdAsync(string id);
    Task<List<GeneratedPassage>> GetByTopicAsync(string topic);
    Task CreateAsync(GeneratedPassage passage);
}