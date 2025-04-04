using FluentAssertions;
using GOCAP.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace GOCAP.Tests.Integration;

public class RoomsControllerTests(WebApplicationFactory<Api.Program> factory) : IClassFixture<WebApplicationFactory<Api.Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    // Test: Get rooms with paging
    [Fact]
    public async Task GetWithPaging_ReturnsOkResult()
    {
        // Arrange
        var queryInfo = new QueryInfo();
        var requestUri = $"/rooms?Top={queryInfo.Top}&Skip={queryInfo.Skip}";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("rooms");
    }

    // Test: Get room by id
    [Fact]
    public async Task GetById_ReturnsOkResult()
    {
        // Arrange
        var roomId = Guid.NewGuid(); 
        var requestUri = $"/rooms/{roomId}";

        // Act
        var response = await _client.GetAsync(requestUri);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        result.Should().Contain("room");
    }
    // Test: Delete a room
    [Fact]
    public async Task DeleteRoom_ReturnsNoContentResult()
    {
        // Arrange
        var roomId = Guid.NewGuid(); // Thay bằng một RoomId hợp lệ
        var requestUri = $"/rooms/{roomId}";

        // Act
        var response = await _client.DeleteAsync(requestUri);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
