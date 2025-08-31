using Azure.Data.Tables;
using Azure.Identity;

namespace Backend.Infrastructure;

internal class TranslationsService
{
    private readonly TableClient tableClient;
    private const string TableName = "TranslationRequests";
    private bool isTableCreated = false;
    public TranslationsService(IConfiguration config, ILogger<TranslationsService> logger) // who needs logging?
    {
        var connString = config["TableStorageAccount"] ?? throw new Exception("Cannot initialize without a connectionstring");

        if (connString == "UseDevelopmentStorage=true;")
            tableClient = new(connString, TableName);

        else
            tableClient = new TableClient(new Uri(connString), TableName, new DefaultAzureCredential());

        // this is only done because its a simple case/prototype, never allow async in ctor.
        CreateTableIfNotExistsAsync(); 
    }

    public async Task AddTranslationAsync(TranslationEntity entity)
    {
        if (!isTableCreated)
            await CreateTableIfNotExistsAsync();
        await tableClient.AddEntityAsync(entity);
    }

    private async Task CreateTableIfNotExistsAsync()
    {
        try
        {
            await tableClient.CreateIfNotExistsAsync();
            isTableCreated = true;
        }

        catch (Exception _)
        {
        }
    }

}
