namespace GOCAP.Tests.Unit;

public class StoryServiceTests
{
    private readonly Mock<IStoryRepository> _repositoryMock;
    private readonly Mock<IStoryReactionRepository> _storyReactionRepositoryMock;
    private readonly Mock<IStoryViewRepository> _storyViewRepositoryMock;
    private readonly Mock<IStoryHighlightRepository> _storyHighlightRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IKafkaProducer> _kafkaProducerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly StoryService _storyService;

    public StoryServiceTests()
    {
        _repositoryMock = new Mock<IStoryRepository>();
        _storyReactionRepositoryMock = new Mock<IStoryReactionRepository>();
        _storyViewRepositoryMock = new Mock<IStoryViewRepository>();
        _storyHighlightRepositoryMock = new Mock<IStoryHighlightRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _kafkaProducerMock = new Mock<IKafkaProducer>();
        _mapperMock = new Mock<IMapper>();

        _storyService = new StoryService(
            _repositoryMock.Object,
            _storyReactionRepositoryMock.Object,
            _storyViewRepositoryMock.Object,
            _storyHighlightRepositoryMock.Object,
            _userRepositoryMock.Object,
            _blobStorageServiceMock.Object,
            _unitOfWorkMock.Object,
            _kafkaProducerMock.Object,
            _mapperMock.Object,
            NullLogger<StoryService>.Instance
        );
    }

    #region AddAsync

    [Fact]
    public async Task AddAsync_ShouldThrowResourceNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var story = new Story
        {
            UserId = Guid.NewGuid(),
            MediaUpload = new MediaUpload { FileName = "test.jpg" }
        };

        _userRepositoryMock.Setup(u => u.CheckExistAsync(story.UserId)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(() => _storyService.AddAsync(story));
        Assert.Equal($"User {story.UserId} was not found.", exception.Message);
        _userRepositoryMock.Verify(u => u.CheckExistAsync(story.UserId), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<StoryEntity>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ShouldThrowInternalException_WhenUnexpectedErrorOccurs()
    {
        // Arrange
        var story = new Story
        {
            UserId = Guid.NewGuid(),
            MediaUpload = new MediaUpload { FileName = "test.jpg" }
        };
        var media = new Media { Url = "https://example.com/media" };
        var storyEntity = new StoryEntity { Id = Guid.NewGuid(), UserId = story.UserId };

        _userRepositoryMock.Setup(u => u.CheckExistAsync(story.UserId)).ReturnsAsync(true);
        _blobStorageServiceMock.Setup(b => b.UploadFileAsync(story.MediaUpload)).ReturnsAsync(media);
        _mapperMock.Setup(m => m.Map<StoryEntity>(story)).Returns(storyEntity);
        _repositoryMock.Setup(r => r.AddAsync(storyEntity)).ThrowsAsync(new Exception("Unexpected error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InternalException>(() => _storyService.AddAsync(story));
        Assert.Equal("Unexpected error", exception.Message);
        _repositoryMock.Verify(r => r.AddAsync(storyEntity), Times.Once);
    }

    #endregion
}