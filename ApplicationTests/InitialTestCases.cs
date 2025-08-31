
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace ApplicationTests; 

public class InitialTestCases : WebApplicationFactory<Program>
{
    private const string route = "/translate";
    HttpClient client;

    public InitialTestCases()
    {
        client = CreateClient();
        client.DefaultRequestHeaders.Add("x-mySecret", "mySecret");

    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
    }



    [Fact]
    public async Task WhenValidReponseIsMade_ThenSuccessIsReturned()
    {
        // When
        var body = new {
            Text = "Hello World",
            ImageId =  "image:" + Guid.NewGuid(),
            CountryCode = "DK",
            TextAlignment = "Right"
        };
        var response = await client.PostAsJsonAsync(route, body);

        //Then
        var response1 = await client.PostAsJsonAsync(route, body with {CountryCode="GB" });
        response.EnsureSuccessStatusCode();
    }

    [Fact] public async Task WhenInvalidCountryCodeIsUsed_ThenBadRequestIsReturned()
    {
               // When
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
    
    // write more tests.
}