namespace Practice.Infrastructure.Settings;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string PassagesCollectionName { get; set; } = null!;
    public string SpeakingCollectionName { get; set; } = null!;
    public string EssaysCollectionName { get; set; } = null!;
}
