using FluentAssertions;
using GOCAP.Api.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GOCAP.Tests.Integration;

public class RoomsControllerTests(WebApplicationFactory<Api.Program> factory) : IClassFixture<WebApplicationFactory<Api.Program>>
{
    private readonly HttpClient _client = factory.CreateClient();
    private Guid _testRoomId;
    private readonly string _accessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJwaWN0dXJlIjoie1wiVXJsXCI6XCJodHRwczovL3N0b3JhZ2UuZGVyYXBpLmlvLnZuL2NhcGNhcC9nb2NhcC9hZDdhM2M5ZC0zMWU4LTQwYjQtODgxNy00NzhjZGUzODRmMWMtMjAyNTAzMTAxODA5LUltYWdlLnBuZ1wiLFwiVHlwZVwiOjEsXCJUaHVtYm5haWxVcmxcIjpudWxsfSIsInVuaXF1ZV9uYW1lIjoiR8OidSBHw6J1IiwiZW1haWwiOiJicmlnaHRzdW50bmMyMDAzQGdtYWlsLmNvbSIsImlkIjoiOWQ4NWU0OWMtYjZiNi00OWRlLTg2Y2EtNTJlYzExMzZlY2ZlIiwicm9sZSI6IlVzZXIiLCJuYmYiOjE3NDM5NjYxMzUsImV4cCI6MTc0Mzk3MzMzNSwiaWF0IjoxNzQzOTY2MTM1LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MDM1IiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzAzNSJ9.vEHUMOZ31kgJXDaRiiumpFd8u7CJh8Qpms8Xlp0kobElj7WTp5VA8j9Opr19Fq6b4lm_VK3dkTh8SE37fth0kw";
    [Fact]
    public async Task GetWithPaging_ReturnsOkResult()
    {
        // Arrange
        var requestUri = "/rooms?Top=10&Skip=0";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        // Arrange
        var nonExistentRoomId = Guid.NewGuid();
        var requestUri = $"/rooms/{nonExistentRoomId}";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        Assert.Equal(404, (int)response.StatusCode);
    }

    [Fact]
    public async Task CreateRoom_ShouldReturnCreatedRoom_WhenValidRequest()
    {
        var model = new RoomCreationModel
        {
            Topic = $"Test Room {Guid.NewGuid()}",
            Description = "This is a test room",
            MaximumMembers = 10
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_accessToken}");

        var createResponse = await _client.PostAsJsonAsync("/rooms", model);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createdRoom = await createResponse.Content.ReadFromJsonAsync<RoomModel>();
        createdRoom.Should().NotBeNull();

        _testRoomId = createdRoom!.Id; 
    }

    [Fact]
    public async Task DeleteRoom_ShouldReturnNotFound_WhenRoomDoesNotExist()
    {
        var nonExistentRoomId = Guid.NewGuid();
        var requestUri = $"/rooms/{nonExistentRoomId}";

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_accessToken}");

        var response = await _client.DeleteAsync(requestUri);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRoom_ShouldReturnUpdatedRoom_WhenValidRequest()
    {
        await CreateRoom_ShouldReturnCreatedRoom_WhenValidRequest();

        var updatedModel = new RoomUpdationModel
        {
            Topic = $"Updated Room {Guid.NewGuid()}",
            Description = "This is an updated test room",
            MaximumMembers = 20
        };

        var requestUri = $"/rooms/{_testRoomId}";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_accessToken}");

        var updateResponse = await _client.PatchAsJsonAsync(requestUri, updatedModel);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateAndDeleteRoom_ShouldSucceed()
    {
        await CreateRoom_ShouldReturnCreatedRoom_WhenValidRequest();
        var deleteResponse = await _client.DeleteAsync($"/rooms/{_testRoomId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK); 
    }
}
