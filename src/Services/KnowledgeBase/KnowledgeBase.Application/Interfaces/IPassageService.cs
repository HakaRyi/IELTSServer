using System.Collections.Generic;
using System.Threading.Tasks;
using KnowledgeBase.Application.DTOs;
using KnowledgeBase.Domain.Entities;

namespace KnowledgeBase.Application.Interfaces;

public interface IPassageService
{
    Task<GeneratedPassage> GenerateAndSavePassageAsync(GeneratePassageDto dto);
    Task<List<GeneratedPassage>> GetPassagesByTopicAsync(string topic);
}