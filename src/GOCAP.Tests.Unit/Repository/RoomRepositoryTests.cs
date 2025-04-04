namespace GOCAP.Tests.Unit;

public class RoomRepositoryTests : RepositoryBaseTests
{
    private readonly RoomRepository _roomRepository;
    public RoomRepositoryTests()
    {
        _roomRepository = new RoomRepository(_dbContext);
    }

    #region GetDetailByIdAsync
    [Fact]
    public async Task GetDetailByIdAsync_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        // Arrange
        var roomId = Guid.NewGuid(); 

        // Act
        var result = await _roomRepository.GetDetailByIdAsync(roomId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetDetailByIdAsync_ShouldReturnNullOwner_WhenUserDoesNotExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var roomEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(), // OwnerId does not exist
            Topic = "TestRoom",
            Description = "Test Description",
            MaximumMembers = 10,
            Medias = "[]",
            Status = RoomStatusType.Available,
            CreateTime = DateTime.UtcNow.Ticks
        };

        _dbContext.Rooms.Add(roomEntity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _roomRepository.GetDetailByIdAsync(roomId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Owner); // Owner must be null
    }

    [Fact]
    public async Task GetDetailByIdAsync_ShouldReturnEmptyMembers_WhenNoMembersExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var roomEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(),
            Topic = "TestRoom",
            Description = "Test Description",
            MaximumMembers = 10,
            Medias = "[]",
            Status = RoomStatusType.Available,
            CreateTime = DateTime.UtcNow.Ticks
        };

        _dbContext.Rooms.Add(roomEntity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _roomRepository.GetDetailByIdAsync(roomId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Members); 
    }

    [Fact]
    public async Task GetDetailByIdAsync_ShouldReturnMembers_WhenMembersExist()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var roomEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(),
            Topic = "TestRoom",
            Description = "Test Description",
            MaximumMembers = 10,
            Medias = "[]",
            Status = RoomStatusType.Available,
            CreateTime = DateTime.UtcNow.Ticks
        };

        _dbContext.Rooms.Add(roomEntity);
        await _dbContext.SaveChangesAsync();

        var roomMember = new RoomMemberEntity
        {
            RoomId = roomId,
            UserId = userId
        };
        _dbContext.RoomMembers.Add(roomMember);
        await _dbContext.SaveChangesAsync();

        var userEntity = new UserEntity
        {
            Id = userId,
            Name = "Test User",
            Picture = "{}" 
        };
        _dbContext.Users.Add(userEntity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _roomRepository.GetDetailByIdAsync(roomId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Members);
        Assert.Equal("Test User", result.Members.First().Name); 
    }

    [Fact]
    public async Task GetDetailByIdAsync_ShouldHandleInvalidMedias()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var roomEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(),
            Topic = "TestRoom",
            Description = "Test Description",
            MaximumMembers = 10,
            Medias = "InvalidJson", 
            Status = RoomStatusType.Available,
            CreateTime = DateTime.UtcNow.Ticks
        };

        _dbContext.Rooms.Add(roomEntity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _roomRepository.GetDetailByIdAsync(roomId);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Medias); 
    }

    [Fact]
    public async Task GetDetailByIdAsync_ShouldReturnCorrectMaximumMembers()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var roomEntity = new RoomEntity
        {
            Id = roomId,
            OwnerId = Guid.NewGuid(),
            Topic = "TestRoom",
            Description = "Test Description",
            MaximumMembers = 50, 
            Medias = "[]",
            Status = RoomStatusType.Available,
            CreateTime = DateTime.UtcNow.Ticks
        };

        _dbContext.Rooms.Add(roomEntity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _roomRepository.GetDetailByIdAsync(roomId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(50, result.MaximumMembers); 
    }
    #endregion
}
