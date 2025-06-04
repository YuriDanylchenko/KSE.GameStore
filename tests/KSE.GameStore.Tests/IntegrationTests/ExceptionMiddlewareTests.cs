using KSE.GameStore.ApplicationCore.Infrastructure;
using KSE.GameStore.ApplicationCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace KSE.GameStore.Tests.IntegrationTests;

public class ExceptionMiddlewareTests
{
    private TestServer CreateServerThatThrows(Exception ex)
    {
        return new TestServer(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging();
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

    private async Task<Error> GetErrorResponse(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<Error>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    [Fact]
    public async Task Handles_BadRequestException_Returns_400()
    {
        var ex = new BadRequestException("Bad request!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Bad request!", error.Message);
        Assert.Equal(400, error.Status);
    }

    [Fact]
    public async Task Handles_UnauthorizedException_Returns_401()
    {
        var ex = new UnauthorizedException("Unauthorized access!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Unauthorized access!", error.Message);
        Assert.Equal(401, error.Status);
    }

    [Fact]
    public async Task Handles_ForbiddenException_Returns_403()
    {
        var ex = new ForbiddenException("Forbidden access!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Forbidden access!", error.Message);
        Assert.Equal(403, error.Status);
    }

    [Fact]
    public async Task Handles_NotFoundException_Returns_404()
    {
        var ex = new NotFoundException("Not found!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Not found!", error.Message);
        Assert.Equal(404, error.Status);
    }

    [Fact]
    public async Task Handles_InternalServerErrorException_Returns_500()
    {
        var ex = new InternalServerErrorException("Internal failure!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Internal failure!", error.Message);
        Assert.Equal(500, error.Status);
    }

    [Fact]
    public async Task Handles_UnexpectedException_Returns_500()
    {
        var ex = new Exception("Unexpected error!");
        using var server = CreateServerThatThrows(ex);
        var client = server.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

        var error = await GetErrorResponse(response);
        Assert.NotNull(error);
        Assert.Equal("Unexpected error!", error.Message);
        Assert.Equal(500, error.Status);
    }
}
