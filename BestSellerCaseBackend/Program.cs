using Backend;
using Backend.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using static Backend.Constants;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.AllowReadingFromString;
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
builder.Services.AddSingleton<TE>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
//app.Use(async (context, next) =>
//{
//    if (HttpMethods.IsGet(context.Request.Method))    
//        await next();    
//    else
//    {
//        if (context.Request.Headers.TryGetValue("x-mySecret", out var secret) && secret == "mySecret")
//            await next();
//        else
//        {
//            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//            return;
//        }
//    }
//});

app.MapGet(string.Empty, ([FromServices]IConfiguration config) => config.GetConnectionString("TableStorageAccount")).WithOpenApi();
app.MapPost("/translate", async (TranslationRequest request, [FromServices] TranslationsService service) =>
{
    try
    {
        var entity = request.MapToEntity();
        await service.AddTranslationAsync(entity);
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.UnprocessableEntity(e.ToString());
    }

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

public class TE
{
    public List<string> errs = new();
}

