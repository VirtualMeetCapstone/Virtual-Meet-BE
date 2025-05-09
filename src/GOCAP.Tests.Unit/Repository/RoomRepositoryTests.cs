﻿namespace GOCAP.Tests.Unit;

public class RoomRepositoryTests : RepositoryTestsBase
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

    #region GetWithPagingAsync
    [Fact]
    public async Task GetWithPagingAsync_ShouldReturnAllRooms_WhenNoSearchText()
    {
        // Arrange
        var ownerId1 = Guid.NewGuid();
        var ownerId2 = Guid.NewGuid();

        _dbContext.Users.AddRange(
            new UserEntity { Id = ownerId1, Name = "Owner A", Picture = "{}" },
            new UserEntity { Id = ownerId2, Name = "Owner B", Picture = "{}" }
        );

        var roomA = new RoomEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId1,
            Topic = "Room A",
            CreateTime = DateTime.UtcNow.AddMinutes(-1).Ticks,
            Status = RoomStatusType.Available,
            Medias = "[]"
        };

        var roomB = new RoomEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId2,
            Topic = "Room B",
            CreateTime = DateTime.UtcNow.Ticks,
            Status = RoomStatusType.Available,
            Medias = "[]"
        };

        _dbContext.Rooms.AddRange(roomA, roomB);
        await _dbContext.SaveChangesAsync();

        var queryInfo = new QueryInfo
        {
            Skip = 0,
            Top = 10,
            NeedTotalCount = true
        };

        // Act
        var result = await _roomRepository.GetWithPagingAsync(queryInfo);

        // Assert
        Assert.Equal(2, result.Data.Count());
        Assert.Equal(2, result.TotalCount);

        // Check ordering by CreateTime descending
        Assert.Equal("Room B", result.Data.FirstOrDefault()?.Topic);
        Assert.Equal("Room A", result.Data.Skip(1).FirstOrDefault()?.Topic);

        // Check owner not null
        Assert.All(result.Data, r =>
        {
            Assert.NotNull(r.Owner);
            Assert.False(string.IsNullOrEmpty(r.Owner.Name));
        });
    }

    [Fact]
    public async Task GetWithPagingAsync_ShouldReturnCorrectTotalCount_WhenNeeded()
    {
        // Arrange
        var room1 = new RoomEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Topic = "Room A",
            CreateTime = DateTime.UtcNow.Ticks,
            Status = RoomStatusType.Available,
            Medias = "[]"
        };
        var room2 = new RoomEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = Guid.NewGuid(),
            Topic = "Room B",
            CreateTime = DateTime.UtcNow.Ticks,
            Status = RoomStatusType.Available,
            Medias = "[]"
        };
        _dbContext.Rooms.Add(room1);
        _dbContext.Rooms.Add(room2);
        await _dbContext.SaveChangesAsync();

        var queryInfo = new QueryInfo
        {
            NeedTotalCount = true
        };

        // Act
        var result = await _roomRepository.GetWithPagingAsync(queryInfo);

        // Assert
        Assert.Equal(2, result.TotalCount);
    }
    #endregion
}
