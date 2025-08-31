using Azure.Data.Tables;
using Azure.Identity;

namespace Backend.Infrastructure;

internal class TranslationsService
{
    private readonly TableClient tableClient;
    private const string TableName = "TranslationRequests";
    private bool isTableCreated = false;
    TE t;

    public TranslationsService(IConfiguration config, TE t) // who needs logging?
    {
        this.t = t;
        string connectionstring = string.Empty;
        try
        {
            connectionstring = config.GetConnectionString("TableStorageAccount") ?? throw new Exception("Cannot initialize without a connectionstring");

            if (connectionstring == "UseDevelopmentStorage=true;")
            {
                tableClient = new(connectionstring, TableName);
                CreateTableIfNotExistsAsync(); // this is only done because its a simple case/prototype, never allow async in ctor.
            }
            else

                tableClient = new TableClient(new Uri(connectionstring), TableName, new DefaultAzureCredential());

        }
        catch (Exception e)
        {
            t.errs.Add(e.ToString());
            throw;
        }
    }

    public async Task AddTranslationAsync(TranslationEntity entity)
    {
        await tableClient.AddEntityAsync(entity);
    }

    private async Task CreateTableIfNotExistsAsync()
    {
        try
        {
            await tableClient.CreateIfNotExistsAsync();
            isTableCreated = true;
        }

        catch (Exception e)
        {
            t.errs.Add(e.ToString());
        }
    }

}
