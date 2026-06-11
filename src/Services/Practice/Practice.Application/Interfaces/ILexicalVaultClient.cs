namespace Practice.Application.Interfaces;

public interface ILexicalVaultClient
{
    Task<List<string>> GetWordValuesByIdsAsync(string userId, List<string> ids);
    /// <summary>Lấy tất cả từ thuộc topic của user — dùng khi sinh bài nói không chọn từ cụ thể</summary>
    Task<List<string>> GetWordValuesByTopicAsync(string userId, string topic);
}
