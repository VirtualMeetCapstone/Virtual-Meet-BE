using GOCAP.Services.Intention;
using Microsoft.AspNetCore.Mvc;

namespace GOCAP.Services;

public interface IUserBlockService : IServiceBase<UserBlock>
{
	Task<OperationResult> BlockOrUnblockAsync(UserBlock domain);
	Task<List<UserBlock>> GetUserBlockAsync(Guid userId);
}
