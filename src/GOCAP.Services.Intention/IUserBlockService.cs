using GOCAP.Services.Intention;

namespace GOCAP.Services;

public interface IUserBlockService : IServiceBase<UserBlock>
{
	Task<OperationResult> BlockOrUnblockAsync(UserBlock domain);
}
