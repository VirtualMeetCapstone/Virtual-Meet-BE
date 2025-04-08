namespace GOCAP.Tests.Unit;

public class FollowServiceTests
{
    private readonly Mock<IUserContextService> _mockUserContextService;
    private readonly Mock<IFollowRepository> _mockFollowRepository;
    private readonly Mock<IKafkaProducer> _mockKafkaProducer;
    private readonly Mock<IMapper> _mockMapper;

    private readonly FollowService _followService;

    public FollowServiceTests()
    {
        _mockUserContextService = new Mock<IUserContextService>();
        _mockFollowRepository = new Mock<IFollowRepository>();
        _mockKafkaProducer = new Mock<IKafkaProducer>();
        _mockMapper = new Mock<IMapper>();

        _mockUserContextService.Setup(x => x.Id).Returns(Guid.NewGuid());

        _followService = new FollowService(
            _mockUserContextService.Object,
            _mockFollowRepository.Object,
            _mockKafkaProducer.Object,
            _mockMapper.Object,
            NullLogger<FollowService>.Instance
        );
    }

    #region FollowOrUnfollowAsync
    [Fact]
    public async Task FollowOrUnfollowAsync_ShouldUnfollow_WhenAlreadyFollowing()
    {
        // Arrange
        var followerId = _mockUserContextService.Object.Id;
        var followingId = Guid.NewGuid();

        var existingFollow = new UserFollowEntity
        {
            Id = Guid.NewGuid(),
            FollowerId = followerId,
            FollowingId = followingId
        };

        _mockFollowRepository
            .Setup(r => r.GetByFollowerAndFollowingAsync(followerId, followingId))
            .ReturnsAsync(existingFollow);

        _mockFollowRepository
            .Setup(r => r.DeleteByIdAsync(existingFollow.Id))
            .ReturnsAsync(1);

        var domain = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        // Act
        var result = await _followService.FollowOrUnfollowAsync(domain);

        // Assert
        result.Success.Should().BeTrue();
        _mockFollowRepository.Verify(r => r.DeleteByIdAsync(existingFollow.Id), Times.Once);
        _mockKafkaProducer.Verify(p => p.ProduceAsync(It.IsAny<string>(), It.IsAny<NotificationEvent>(), default), Times.Never);
    }

    [Fact]
    public async Task FollowOrUnfollowAsync_ShouldThrowException_WhenFollowerEqualsFollowing()
    {
        var userId = Guid.NewGuid();
        _mockUserContextService.Setup(x => x.Id).Returns(userId);

        var domain = new Follow
        {
            FollowerId = userId,
            FollowingId = userId
        };

        Func<Task> act = async () => await _followService.FollowOrUnfollowAsync(domain);

        await act.Should().ThrowAsync<ParameterInvalidException>();
    }

    [Fact]
    public async Task FollowOrUnfollowAsync_ShouldFollow_WhenNotAlreadyFollowing()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();

        var followDomain = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        var mappedEntity = new UserFollowEntity
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        _mockUserContextService.Setup(x => x.Id).Returns(followerId);
        _mockFollowRepository.Setup(x => x.GetByFollowerAndFollowingAsync(followerId, followingId))
            .ReturnsAsync((UserFollowEntity?)null);

        _mockMapper.Setup(x => x.Map<UserFollowEntity>(It.IsAny<Follow>())).Returns(mappedEntity);
        _mockFollowRepository.Setup(x => x.AddAsync(It.IsAny<UserFollowEntity>()))
                                .ReturnsAsync((UserFollowEntity entity) => entity);


        _mockKafkaProducer.Setup(k => k.ProduceAsync(
            KafkaConstants.Topics.Notification,
            It.IsAny<NotificationEvent>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask);

        // Act
        var result = await _followService.FollowOrUnfollowAsync(followDomain);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        _mockFollowRepository.Verify(r => r.AddAsync(It.Is<UserFollowEntity>(e =>
            e.FollowerId == followerId && e.FollowingId == followingId
        )), Times.Once);

        _mockKafkaProducer.Verify(k => k.ProduceAsync(KafkaConstants.Topics.Notification,
            It.Is<NotificationEvent>(n =>
                n.Type == NotificationType.Follow &&
                n.ActionType == ActionType.Add &&
                n.UserId == followingId &&
                n.ActorId == followerId
            ),
            It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region IsFollowingAsync
    [Fact]
    public async Task IsFollowingAsync_ShouldReturnTrue_WhenUserIsFollowing()
    {
        // Arrange
        var followingId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.Id).Returns(currentUserId);

        _mockFollowRepository
            .Setup(r => r.IsFollowingAsync(It.Is<Follow>(
                f => f.FollowerId == currentUserId && f.FollowingId == followingId)))
            .ReturnsAsync(true);

        // Act
        var result = await _followService.IsFollowingAsync(followingId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFollowingAsync_ShouldReturnFalse_WhenUserIsNotFollowing()
    {
        // Arrange
        var followingId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid();

        _mockUserContextService.Setup(x => x.Id).Returns(currentUserId);

        _mockFollowRepository
            .Setup(r => r.IsFollowingAsync(It.Is<Follow>(
                f => f.FollowerId == currentUserId && f.FollowingId == followingId)))
            .ReturnsAsync(false);

        // Act
        var result = await _followService.IsFollowingAsync(followingId);

        // Assert
        result.Should().BeFalse();
    }
    #endregion
}
