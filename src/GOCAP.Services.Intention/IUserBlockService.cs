using GOCAP.Services.Intention;
using Microsoft.AspNetCore.Mvc;

namespace GOCAP.Services;

public interface IUserBlockService
{
	Task<OperationResult> BlockOrUnblockAsync(UserBlock domain);
	Task<List<UserBlock>> GetUserBlocksAsync(Guid userId);
}
