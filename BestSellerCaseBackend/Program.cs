using Azure.Identity;
using Backend;
using Backend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static Backend.Constants;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureAppConfiguration(options =>
    {
        options.Connect(new Uri(builder.Configuration["AppConfigEndpoint"]), new DefaultAzureCredential())
        .Select("UserService*").TrimKeyPrefix("UserService:");
    });
}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // Allows both string and numeric enum values
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.NumberHandling =
            System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddLogging();
builder.Services.AddTransient<TranslationsService>();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();



app.MapGet(string.Empty, () => "hello world").WithOpenApi();
app.MapPost("/translate", async (TranslationRequest request, [FromServices] TranslationsService service) =>
{
    //handle dublicates or allow multiple translations per image?
    var entity = request.MapToEntity();
    await service.AddTranslationAsync(entity);
    return Results.Ok();
})
.WithOpenApi();

app.Run();
public partial class Program { }


internal record TranslationRequest
{
    public string Text { get; init; } = string.Empty;
    public string ImageId { get; init; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CountryCode CountryCode { get; init; } = CountryCode.Undefined;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TextAlignment TextAlignment { get; init; } = TextAlignment.Undefined;
}


