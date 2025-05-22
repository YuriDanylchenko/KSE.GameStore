using KSE.GameStore.ApplicationCore.Models;
using KSE.GameStore.ApplicationCore.Responses;
using KSE.GameStore.Web.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace KSE.GameStore.Tests;

public class ExceptionMiddlewareTests
{
    private TestServer CreateServerThatThrows(Exception ex)
    {
        return new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging();
                services.AddScoped<ExceptionMiddleware>();
            })
            .Configure(app =>
            {
                app.UseMiddleware<ExceptionMiddleware>();

                app.Use((HttpContext _, Func<Task> _) =>
                {
                    throw ex;
                });
            }));
    }

    private async Task<ErrorResponse> GetErrorResponse(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ErrorResponse>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    [Fact]
    public async Task Handles_ServerException_Returns_StatusCodeAndMessage()
    {
        // Arrange
        var ex = new ServerException("Server error occurred", 401);
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert status code
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        // Assert response body content
        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Server error occurred", error.Message);
        Assert.Equal(401, error.Status);
    }

    [Fact]
    public async Task Handles_NullReferenceException_Returns_404()
    {
        // Arrange
        var ex = new NullReferenceException("Null reference!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert status code
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        // Assert response body content
        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Null reference!", error.Message);
        Assert.Equal(404, error.Status);
    }

    [Fact]
    public async Task Handles_UnexpectedException_Returns_500()
    {
        // Arrange
        var ex = new Exception("Unexpected error!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert status code
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        // Assert response body content
        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Unexpected error!", error.Message);
        Assert.Equal(500, error.Status);
    }
}
