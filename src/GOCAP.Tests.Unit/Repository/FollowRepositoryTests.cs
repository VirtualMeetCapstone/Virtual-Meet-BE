namespace GOCAP.Tests.Unit;

public class FollowRepositoryTests : RepositoryTestsBase
{
    private readonly FollowRepository _repository;
    public FollowRepositoryTests()
    {
        _repository = new FollowRepository(_dbContext);
    }

    #region GetByFollowerAndFollowingAsync
    [Fact]
    public async Task GetByFollowerAndFollowingAsync_ShouldReturnEntity_WhenExists()
    {
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();

        _dbContext.UserFollows.Add(new UserFollowEntity
        {
            Id = Guid.NewGuid(),
            FollowerId = followerId,
            FollowingId = followingId,
            CreateTime = DateTime.UtcNow.Ticks
        });
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetByFollowerAndFollowingAsync(followerId, followingId);

        Assert.NotNull(result);
        Assert.Equal(followerId, result!.FollowerId);
        Assert.Equal(followingId, result.FollowingId);
    }

    [Fact]
    public async Task GetByFollowerAndFollowingAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByFollowerAndFollowingAsync(Guid.NewGuid(), Guid.NewGuid());
        Assert.Null(result);
    }
    #endregion

    #region GetFollowersByUserIdAsync
    [Fact]
    public async Task GetFollowersByUserIdAsync_ShouldReturnCorrectFollowers()
    {
        var userId = Guid.NewGuid();
        var follower1 = Guid.NewGuid();
        var follower2 = Guid.NewGuid();

        _dbContext.UserFollows.AddRange(
            new UserFollowEntity { Id = Guid.NewGuid(), FollowerId = follower1, FollowingId = userId },
            new UserFollowEntity { Id = Guid.NewGuid(), FollowerId = follower2, FollowingId = userId }
        );
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetFollowersByUserIdAsync(userId);

        Assert.Equal(2, result.Count);
        Assert.Contains(follower1, result);
        Assert.Contains(follower2, result);
    }

    [Fact]
    public async Task GetFollowersByUserIdAsync_ShouldReturnEmptyList_WhenNoFollowers()
    {
        var result = await _repository.GetFollowersByUserIdAsync(Guid.NewGuid());
        Assert.Empty(result);
    }
    #endregion

    #region IsFollowingAsync
    [Fact]
    public async Task IsFollowingAsync_ShouldReturnTrue_WhenExists()
    {
        var followerId = Guid.NewGuid();
        var followingId = Guid.NewGuid();

        _dbContext.UserFollows.Add(new UserFollowEntity
        {
            Id = Guid.NewGuid(),
            FollowerId = followerId,
            FollowingId = followingId
        });
        await _dbContext.SaveChangesAsync();

        var domain = new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        var result = await _repository.IsFollowingAsync(domain);

        Assert.True(result);
    }

    [Fact]
    public async Task IsFollowingAsync_ShouldReturnFalse_WhenNotExists()
    {
        var domain = new Follow
        {
            FollowerId = Guid.NewGuid(),
            FollowingId = Guid.NewGuid()
        };

        var result = await _repository.IsFollowingAsync(domain);

        Assert.False(result);
    }
    #endregion
}
