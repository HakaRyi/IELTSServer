namespace Practice.Application.Interfaces;

public interface ILexicalVaultClient
{
    Task<List<string>> GetWordValuesByIdsAsync(List<string> ids);
    /// <summary>Lấy tất cả từ thuộc topic — dùng khi sinh bài nói không chọn từ cụ thể</summary>
    Task<List<string>> GetWordValuesByTopicAsync(string topic);
}
