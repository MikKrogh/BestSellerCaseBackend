namespace Backend;

public static class Constants
{
    public enum CountryCode
    {
        Undefined = 0,
        US,
        UK,
        DK,
        FR,
        DE,
        IT,
        ES,
        JP,
        CN
    }
    public enum TextAlignment
    {
        Undefined = 0,
        Left,
        Center,
        Right,
        Justify,
        Top,
        Bottom
    }
}
internal static class EntityHelper
{
    internal static TranslationEntity MapToEntity(this TranslationRequest request)
    {
        if (request.CountryCode is Constants.CountryCode.Undefined || request.TextAlignment is Constants.TextAlignment.Undefined) throw new BadHttpRequestException("invalid coutry code or text alignment.", 400);

        return new TranslationEntity
        {
            PartitionKey = request.ImageId,
            RowKey = request.CountryCode.ToString(),
            Text = request.Text,
            TextAlignment = request.TextAlignment.ToString(),
            IsTranslated = false
        };
    }
}
