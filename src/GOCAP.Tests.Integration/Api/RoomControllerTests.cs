namespace GOCAP.Tests.Integration;

public class RoomsControllerTests(WebApplicationFactory<Api.Program> factory) : ApiControllerTestsBase
{
    private readonly HttpClient _client = factory.CreateClient();
    private Guid _testRoomId;
    
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
    public async Task CreateRoom_ShouldReturnBadRequest_WhenTopicIsMissing()
    {
        // Arrange
        var model = new RoomCreationModel
        {
            Topic = "", 
            Description = "This room has no topic",
            MaximumMembers = 10
        };

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_accessToken}");

        // Act
        var response = await _client.PostAsJsonAsync("/rooms", model);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorModel>();
        error.Should().NotBeNull();

        error.ErrorMessage.Should().Be("Validation failed.");
        error.ErrorDetails.Should().ContainSingle("Topic is required.");
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
