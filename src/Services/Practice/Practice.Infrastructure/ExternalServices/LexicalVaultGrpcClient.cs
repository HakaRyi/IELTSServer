using Practice.Application.Interfaces;
using Practice.Infrastructure.Grpc;

namespace Practice.Infrastructure.ExternalServices;

public class LexicalVaultGrpcClient : ILexicalVaultClient
{
    private readonly LexicalVault.LexicalVaultClient _client;

    public LexicalVaultGrpcClient(LexicalVault.LexicalVaultClient client)
    {
        _client = client;
    }

    public async Task<List<string>> GetWordValuesByIdsAsync(string userId, List<string> ids)
    {
        var response = await _client.GetAllLexicalItemsAsync(
            new GetLexicalRequest { UserId = userId });
        var idSet = new HashSet<string>(ids);
        return response.Items
            .Where(i => idSet.Contains(i.Id))
            .Select(i => i.Value)
            .ToList();
    }

    public async Task<List<string>> GetWordValuesByTopicAsync(string userId, string topic)
    {
        var response = await _client.GetAllLexicalItemsAsync(
            new GetLexicalRequest { UserId = userId });
        return response.Items
            .Where(i => string.IsNullOrEmpty(topic) ||
                        i.Topics.Any(t => t.Equals(topic, StringComparison.OrdinalIgnoreCase)))
            .Select(i => i.Value)
            .Distinct()
            .ToList();
    }
}
