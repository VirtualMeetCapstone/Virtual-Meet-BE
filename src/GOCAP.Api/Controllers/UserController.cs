namespace GOCAP.Api.Controllers;

[Route("users")]
[ApiController]
public class UserController(IUserService _userService, IMapper _mapper) : ApiControllerBase
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
        return await _userService.AddAsync(domain);
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
