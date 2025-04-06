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

    #region AddAsync
    [Fact]
    public async Task AddAsync_ShouldAddRoomSuccessfully_WhenValidDataIsProvided()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Topic = "Test Room",
        };
        var roomEntity = new RoomEntity
        {
            Id = room.Id,
            Topic = room.Topic,
        };

        _blobStorageServiceMock.Setup(b => b.CheckFilesExistByUrlsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(true);

        _mapperMock.Setup(m => m.Map<RoomEntity>(It.IsAny<Room>())).Returns(roomEntity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<RoomEntity>())).ReturnsAsync(roomEntity);
        _mapperMock.Setup(m => m.Map<Room>(It.IsAny<RoomEntity>())).Returns(room);

        // Act
        var result = await _roomService.AddAsync(room);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(room.Id, result.Id);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<RoomEntity>()), Times.Once);
        _kafkaProducerMock.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowException_WhenMediaFileIsInvalid()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Topic = "Test Room",
            Medias = [new Media { Url = "https://example.com/invalid-media" }]
        };

        _blobStorageServiceMock.Setup(b => b.CheckFilesExistByUrlsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(false); // Media URL does not exist

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ParameterInvalidException>(() => _roomService.AddAsync(room));
        Assert.Equal("At least one media file uploaded is invalid.", exception.Message);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<RoomEntity>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ShouldHandleException_WhenRepositoryFails()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            Topic = "Test Room"
        };
        var roomEntity = new RoomEntity
        {
            Id = room.Id,
            Topic = room.Topic
        };

        _blobStorageServiceMock.Setup(b => b.CheckFilesExistByUrlsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(true); // Media URLs are valid

        _mapperMock.Setup(m => m.Map<RoomEntity>(It.IsAny<Room>())).Returns(roomEntity);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<RoomEntity>())).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _roomService.AddAsync(room));
        Assert.Equal("Database error", exception.Message);
        _kafkaProducerMock.Verify(k => k.ProduceAsync(It.IsAny<string>(), It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region UpdateAsync
    [Fact]
    public async Task UpdateAsync_ShouldUpdateRoomSuccessfully_WhenValidDataIsProvided()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var domain = new Room
        {
            Id = roomId,
            Topic = "Updated Room",
            Description = "Updated Description",
            MaximumMembers = 100,
            Medias = [new Media { Url = "https://example.com/media2" }],
        };
        var existingEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = _userContextServiceMock.Object.Id,
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } }),
            Topic = "Old Room",
            Description = "Old Description",
            MaximumMembers = 50,
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(existingEntity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<RoomEntity>())).ReturnsAsync(true);
        _blobStorageServiceMock.Setup(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>())).Returns((Task<bool>)Task.FromResult(true));

        // Act
        var result = await _roomService.UpdateAsync(roomId, domain);

        // Assert
        Assert.True(result.Success);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoomEntity>()), Times.Once);
        _blobStorageServiceMock.Verify(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowForbiddenException_WhenUserIsNotOwner()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var domain = new Room
        {
            Id = roomId,
            Topic = "Updated Room",
            Description = "Updated Description",
            MaximumMembers = 100
        };

        var existingEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(), // Different owner
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } }),
            Topic = "Old Room",
            Description = "Old Description",
            MaximumMembers = 50,
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(existingEntity);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ForbiddenException>(() => _roomService.UpdateAsync(roomId, domain));
        Assert.Equal("You are not the owner of this room.", exception.Message);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoomEntity>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldDeleteOldMedia_WhenNewMediaIsProvided()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var domain = new Room
        {
            Id = roomId,
            Topic = "Updated Room with Media",
            Description = "Updated with media",
            MaximumMembers = 100,
            Medias = [new Media { Url = "https://example.com/media2" }],
        };
        var existingEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = _userContextServiceMock.Object.Id,
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } }),
            Topic = "Old Room",
            Description = "Old Description",
            MaximumMembers = 50,
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(existingEntity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<RoomEntity>())).ReturnsAsync(true);
        _blobStorageServiceMock.Setup(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>())).Returns(Task.FromResult(true));

        // Act
        var result = await _roomService.UpdateAsync(roomId, domain);

        // Assert
        _blobStorageServiceMock.Verify(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<RoomEntity>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldHandleException_WhenUpdateFails()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var domain = new Room
        {
            Id = roomId,
            Topic = "Updated Room",
            Description = "Updated Description",
            MaximumMembers = 100
        };

        var existingEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = _userContextServiceMock.Object.Id,
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } }),
            Topic = "Old Room",
            Description = "Old Description",
            MaximumMembers = 50,
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(existingEntity);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<RoomEntity>())).ThrowsAsync(new Exception("Update failed"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _roomService.UpdateAsync(roomId, domain));
        Assert.Equal("Update failed", exception.Message);
    }
    #endregion

    #region DeleteAsync
    [Fact]
    public async Task DeleteByIdAsync_ShouldDeleteRoomSuccessfully_WhenValidDataIsProvided()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var room = new RoomEntity
        {
            Id = roomId,
            OwnerId = _userContextServiceMock.Object.Id,
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } })
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(room);
        _repositoryMock.Setup(r => r.DeleteByEntityAsync(It.IsAny<RoomEntity>())).ReturnsAsync(1);
        _roomMemberRepositoryMock.Setup(r => r.DeleteByRoomIdAsync(roomId)).ReturnsAsync(1);
        _blobStorageServiceMock.Setup(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync(default)).Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.DeleteByIdAsync(roomId);

        // Assert
        Assert.True(result.Success);
        _repositoryMock.Verify(r => r.DeleteByEntityAsync(It.IsAny<RoomEntity>()), Times.Once);
        _roomMemberRepositoryMock.Verify(r => r.DeleteByRoomIdAsync(roomId), Times.Once);
        _blobStorageServiceMock.Verify(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(default), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldThrowForbiddenException_WhenUserIsNotOwner()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var room = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(), // Different owner
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } })
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(room);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ForbiddenException>(() => _roomService.DeleteByIdAsync(roomId));
        Assert.Equal("You are not the owner of this room.", exception.Message);
        _repositoryMock.Verify(r => r.DeleteByEntityAsync(It.IsAny<RoomEntity>()), Times.Never);
    }

    [Fact]
    public async Task DeleteByIdAsync_ShouldRollbackTransaction_WhenExceptionOccurs()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var room = new RoomEntity
        {
            Id = roomId,
            OwnerId = _userContextServiceMock.Object.Id,
            Medias = JsonHelper.Serialize(new List<Media> { new() { Url = "https://example.com/media1" } })
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(roomId, false)).ReturnsAsync(room);
        _repositoryMock.Setup(r => r.DeleteByEntityAsync(It.IsAny<RoomEntity>())).ThrowsAsync(new Exception("Error deleting room"));
        _roomMemberRepositoryMock.Setup(r => r.DeleteByRoomIdAsync(roomId)).ReturnsAsync(1);
        _blobStorageServiceMock.Setup(b => b.DeleteFilesByUrlsAsync(It.IsAny<List<string>>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(default)).Returns(Task.CompletedTask);

        // Act
        var result = await _roomService.DeleteByIdAsync(roomId);

        // Assert
        Assert.False(result.Success);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(default), Times.Once);
    }
    #endregion
}
