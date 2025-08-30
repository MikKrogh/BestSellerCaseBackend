using Azure;
using Azure.Data.Tables;

internal record TranslationEntity : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty;    
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string Text { get; init; } = string.Empty;
    public string TextAlignment { get; init; } = string.Empty;
    public bool IsTranslated { get; init; } = false;
    public string CountryCode => RowKey;
    public string ImageId => PartitionKey;
}

