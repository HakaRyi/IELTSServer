using Grpc.Core;
using KnowledgeBase.Application.Interfaces;
using KnowledgeBase.API.Grpc;

namespace KnowledgeBase.API.GrpcServices;

public class LexicalGrpcService : LexicalVault.LexicalVaultBase
{
    private readonly ILexicalItemService _service;

    public LexicalGrpcService(ILexicalItemService service)
    {
        _service = service;
    }

    public override async Task<LexicalListResponse> GetAllLexicalItems(
        EmptyRequest request, ServerCallContext context)
    {
        var items = await _service.GetAllAsync();
        var response = new LexicalListResponse();

        foreach (var item in items)
        {
            var msg = new LexicalItemMessage
            {
                Id    = item.Id,
                Value = item.Value,
                Type  = item.Type,
            };
            msg.Topics.AddRange(item.Topics);
            response.Items.Add(msg);
        }

        return response;
    }
}
