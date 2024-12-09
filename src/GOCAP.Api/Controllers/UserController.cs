namespace GOCAP.Api.Controllers;

[Route("users")]
[ApiController]
public class UserController(IUserService _userService, IMediaService _mediaService, IMapper _mapper) : ApiControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<User>> GetAll()
    {
        var domain = await _userService.GetAllAsync();
        return _mapper.Map<List<User>>(domain);
    }

    [HttpGet("page")]
    public async Task<QueryResult<User>> GetByPage([FromQuery] QueryInfo queryInfo)
    {
        var result = await _userService.GetByPageAsync(queryInfo);
        return result;
    }

    [HttpGet("{id}")]
    public async Task<User?> GetById([FromRoute] Guid id)
    {
        var domain = await _userService.GetByIdAsync(id);
        return _mapper.Map<User>(domain);
    }

    [HttpPost]
    public async Task<User> Create([FromBody] UserCreationModel model)
    {
        var domain = _mapper.Map<User>(model);

        using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            // Step 1: Save media to MongoDB
            var mediaResult = await _mediaService.AddAsync(new Media
            {
                Id = Guid.NewGuid(),
                Url = "",
                PostId = Guid.NewGuid(),
            });

            // Step 2: Save user to SQL Server
            var userResult = await _userService.AddAsync(domain);

            // Commit transaction
            transaction.Complete();
        }
        catch (Exception ex)
        {
            // Rollback logic if necessary
            Console.WriteLine($"Transaction failed: {ex.Message}");
        }
        return new User();
    }

    [HttpPatch]
    public async Task<OperationResult> Update(Guid id, [FromBody] UserCreationModel model)
    {
        var domain = _mapper.Map<User>(model);
        return await _userService.UpdateAsync(id, domain);
    }

    [HttpDelete("{id}")]
    public async Task<OperationResult> Delete([FromRoute] Guid id)
    {
        return await _userService.DeleteByIdAsync(id);
    }
}
