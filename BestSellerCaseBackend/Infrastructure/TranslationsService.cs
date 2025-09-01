using Azure.Data.Tables;
using Azure.Identity;

namespace Backend.Infrastructure;

internal class TranslationsService
{
    private readonly TableClient tableClient;
    private const string TableName = "TranslationRequests";
    

    public TranslationsService(IConfiguration config) // who needs logging?
    {
        var connectionstring = config.GetConnectionString("TableStorageAccount") ?? throw new Exception("Cannot initialize without a connectionstring");

        if (connectionstring == "UseDevelopmentStorage=true;")        
            tableClient = new(connectionstring, TableName); 
        else
        tableClient = new TableClient(new Uri(connectionstring), TableName, new DefaultAzureCredential());
    }

    public async Task AddTranslationAsync(TranslationEntity entity)
    {
        await tableClient.AddEntityAsync(entity);
    }
}
