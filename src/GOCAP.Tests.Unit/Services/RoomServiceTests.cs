namespace GOCAP.Tests.Unit;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _repositoryMock;
    private readonly Mock<IRoomMemberRepository> _roomMemberRepositoryMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<IUserContextService> _userContextServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IKafkaProducer> _kafkaProducerMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly RoomService _roomService;

    public RoomServiceTests()
    {
        _repositoryMock = new Mock<IRoomRepository>();
        _roomMemberRepositoryMock = new Mock<IRoomMemberRepository>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _userContextServiceMock = new Mock<IUserContextService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _kafkaProducerMock = new Mock<IKafkaProducer>();
        _mapperMock = new Mock<IMapper>();

        _userContextServiceMock.Setup(u => u.Id).Returns(Guid.NewGuid());

        _roomService = new RoomService(
            _repositoryMock.Object,
            _roomMemberRepositoryMock.Object,
            _blobStorageServiceMock.Object,
            _userContextServiceMock.Object,
            _unitOfWorkMock.Object,
            _kafkaProducerMock.Object,
            _mapperMock.Object,
            NullLogger<RoomService>.Instance
        );
    }

    #region GetWithPagingAsync
    [Fact]
    public async Task GetWithPagingAsync_ShouldNotProduceSearchHistory_WhenSearchTextIsEmpty()
    {
        // Arrange
        var queryInfo = new QueryInfo { SearchText = "" };
        var queryResult = new QueryResult<Room> { Data = [] };

        _repositoryMock.Setup(repo => repo.GetWithPagingAsync(queryInfo))
            .ReturnsAsync(queryResult);

        // Act
        await _roomService.GetWithPagingAsync(queryInfo);

        // Assert
        _kafkaProducerMock.Verify(kafka => kafka.ProduceAsync(
            KafkaConstants.Topics.SearchHistory,
            It.IsAny<SearchHistory>(),
            It.IsAny<CancellationToken>()
        ), Times.Never);
    }

    [Fact]
    public async Task GetWithPagingAsync_ShouldReturnRooms_WhenRepositoryReturnsData()
    {
        // Arrange
        var queryInfo = new QueryInfo { SearchText = "test" };
        var queryResult = new QueryResult<Room> { Data = [new()] };

        _repositoryMock.Setup(repo => repo.GetWithPagingAsync(queryInfo))
            .ReturnsAsync(queryResult);

        // Act
        var result = await _roomService.GetWithPagingAsync(queryInfo);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetWithPagingAsync_ShouldReturnRooms_WhenQueryIsValid()
    {
        // Arrange
        var queryInfo = new QueryInfo { SearchText = "test" };
        var expectedRooms = new QueryResult<Room>
        {
            Data = [new() { Id = Guid.NewGuid(), Topic = "Room A" }]
        };

        _repositoryMock
            .Setup(repo => repo.GetWithPagingAsync(queryInfo))
            .ReturnsAsync(expectedRooms);

        // Act
        var result = await _roomService.GetWithPagingAsync(queryInfo);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Topic.Should().Be("Room A");
    }

    [Fact]
    public async Task GetWithPagingAsync_ShouldHandleException_WhenRepositoryFails()
    {
        // Arrange
        var queryInfo = new QueryInfo { SearchText = "error" };

        _repositoryMock
            .Setup(repo => repo.GetWithPagingAsync(queryInfo))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _roomService.GetWithPagingAsync(queryInfo));
        exception.Message.Should().Be("Database error");
    }
    #endregion

    #region GetDetailByIdAsync
    [Fact]
    public async Task GetDetailByIdAsync_ShouldThrowResourceNotFoundException_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _repositoryMock.Setup(repo => repo.GetDetailByIdAsync(roomId))
                       .ReturnsAsync((Room?)null); 
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _roomService.GetDetailByIdAsync(roomId)
        );

        exception.Message.Should().Be($"Room {roomId} was not found.");
    }

    [Fact]
    public async Task GetDetailByIdAsync_ShouldHandleException_WhenRepositoryFails()
    {
        // Arrange
        var roomId = Guid.NewGuid();

        _repositoryMock
            .Setup(repo => repo.GetDetailByIdAsync(roomId))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _roomService.GetDetailByIdAsync(roomId));
        exception.Message.Should().Be("Database error");
    }
    #endregion
}
