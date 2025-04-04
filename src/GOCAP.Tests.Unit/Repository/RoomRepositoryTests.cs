namespace GOCAP.Tests.Unit
{
    public class RoomRepositoryTests
    {
        private readonly Mock<DbSet<RoomEntity>> _roomDbSetMock;
        private readonly Mock<DbSet<RoomMemberEntity>> _roomMemberDbSetMock;
        private readonly Mock<DbSet<UserEntity>> _userDbSetMock;
        private readonly RoomRepository _roomRepository;
        private readonly Mock<IAppConfiguration> _configurationMock;
        private readonly Mock<DbContextOptions<AppSqlDbContext>> _dbContextOptionsMock;
        private readonly Mock<AppSqlDbContext> _dbContextMock;

        public RoomRepositoryTests()
        {
            // Mock DbSets
            _roomDbSetMock = new Mock<DbSet<RoomEntity>>();
            _roomMemberDbSetMock = new Mock<DbSet<RoomMemberEntity>>();
            _userDbSetMock = new Mock<DbSet<UserEntity>>();

            // Mock IAppConfiguration
            _configurationMock = new Mock<IAppConfiguration>();
            _configurationMock.Setup(cfg => cfg.GetSqlServerConnectionString())
                              .Returns("your_connection_string_here");

            // Mock DbContextOptions
            _dbContextOptionsMock = new Mock<DbContextOptions<AppSqlDbContext>>();

            // Mock AppSqlDbContext with constructor parameters
            _dbContextMock = new Mock<AppSqlDbContext>(_dbContextOptionsMock.Object, _configurationMock.Object);

            // Setup DbSets for DbContext
            _dbContextMock.Setup(db => db.Rooms).Returns(_roomDbSetMock.Object);
            _dbContextMock.Setup(db => db.RoomMembers).Returns(_roomMemberDbSetMock.Object);
            _dbContextMock.Setup(db => db.Users).Returns(_userDbSetMock.Object);

            // Initialize repository
            _roomRepository = new RoomRepository(_dbContextMock.Object);
        }

        [Fact]
        public async Task GetWithPagingAsync_ShouldReturnRooms_WhenSearchTextIsNotEmpty()
        {
            // Arrange: Mock Rooms DbSet
            var rooms = new List<RoomEntity>
            {
                new RoomEntity { Id = Guid.NewGuid(), Topic = "TestRoom", Status = RoomStatusType.Available, CreateTime = DateTime.UtcNow.Ticks }
            }.AsQueryable();

            _roomDbSetMock.As<IQueryable<RoomEntity>>()
                .Setup(m => m.Provider).Returns(rooms.Provider);
            _roomDbSetMock.As<IQueryable<RoomEntity>>()
                .Setup(m => m.Expression).Returns(rooms.Expression);
            _roomDbSetMock.As<IQueryable<RoomEntity>>()
                .Setup(m => m.ElementType).Returns(rooms.ElementType);
            _roomDbSetMock.As<IQueryable<RoomEntity>>()
                .Setup(m => m.GetEnumerator()).Returns(rooms.GetEnumerator());

            // Arrange: Mock RoomMembers DbSet
            var roomMembers = new List<RoomMemberEntity>
            {
                new RoomMemberEntity { RoomId = rooms.First().Id, UserId = Guid.NewGuid() }
            }.AsQueryable();

            _roomMemberDbSetMock.As<IQueryable<RoomMemberEntity>>()
                .Setup(m => m.Provider).Returns(roomMembers.Provider);
            _roomMemberDbSetMock.As<IQueryable<RoomMemberEntity>>()
                .Setup(m => m.Expression).Returns(roomMembers.Expression);
            _roomMemberDbSetMock.As<IQueryable<RoomMemberEntity>>()
                .Setup(m => m.ElementType).Returns(roomMembers.ElementType);
            _roomMemberDbSetMock.As<IQueryable<RoomMemberEntity>>()
                .Setup(m => m.GetEnumerator()).Returns(roomMembers.GetEnumerator());

            // Arrange: Mock Users DbSet
            var users = new List<UserEntity>
            {
                new UserEntity { Id = roomMembers.First().UserId, Name = "User1", Picture = "user1.png" }
            }.AsQueryable();

            _userDbSetMock.As<IQueryable<UserEntity>>()
                .Setup(m => m.Provider).Returns(users.Provider);
            _userDbSetMock.As<IQueryable<UserEntity>>()
                .Setup(m => m.Expression).Returns(users.Expression);
            _userDbSetMock.As<IQueryable<UserEntity>>()
                .Setup(m => m.ElementType).Returns(users.ElementType);
            _userDbSetMock.As<IQueryable<UserEntity>>()
                .Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var queryInfo = new QueryInfo { SearchText = "TestRoom", Skip = 0, Top = 10, NeedTotalCount = true };

            // Act
            var result = await _roomRepository.GetWithPagingAsync(queryInfo);

            // Assert
            result.Data.Should().NotBeEmpty();
            result.Data.FirstOrDefault()?.Topic.Should().Be("TestRoom");
            result.TotalCount.Should().Be(1);
        }
    }
}
