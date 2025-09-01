using Backend.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace ApplicationTests;

public class InitialTestCases : WebApplicationFactory<Program>
{
    private const string route = "/translate";
    HttpClient client;
    private readonly TranslationsService service;
    private bool _HasCompletedSetup = false;

    public InitialTestCases()
    {
        client = CreateClient();
        client.DefaultRequestHeaders.Add("x-mySecret", "mySecret");
        service = Services.GetService<TranslationsService>();       // ci/cd-pipeline is running tests before table is completely set up

    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");        
    }



    [Fact]
    public async Task WhenValidReponseIsMade_ThenSuccessIsReturned()
    {
        // When
        await Setup();
        var body = new
        {
            Text = "Hello World",
            ImageId = "image:" + Guid.NewGuid(),
            CountryCode = "DK",
            TextAlignment = "Right"
        };
        var response = await client.PostAsJsonAsync(route, body);

        //Then
        var response1 = await client.PostAsJsonAsync(route, body with { CountryCode = "GB" });
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task WhenInvalidCountryCodeIsUsed_ThenBadRequestIsReturned()
    {
        // When
        await Setup();
        var body = new
        {
            Text = "Hello World",
            ImageId = "image:" + Guid.NewGuid(),
            CountryCode = "InvalidCountry",
            TextAlignment = "Right"
        };
        var response = await client.PostAsJsonAsync(route, body);
        //Then
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }


    private async Task Setup()
    {
        int retryAttemps = 0;
        while (!_HasCompletedSetup && retryAttemps < 5)
        {
            retryAttemps++;
            await service.AttemptCreateTable();

        }
    }

    // write more tests.
}