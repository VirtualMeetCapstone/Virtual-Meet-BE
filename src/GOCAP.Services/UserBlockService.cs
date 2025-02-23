using Microsoft.AspNetCore.Mvc;

namespace GOCAP.Services;

[RegisterService(typeof(IUserBlockService))]
internal class UserBlockService
	(IUserBlockRepository _repository,
	IUserRepository _userRepository,
	IMapper _mapper,
	ILogger<UserBlockService> _logger)
	: ServiceBase<UserBlock, UserBlockEntity>(_repository, _mapper, _logger), IUserBlockService
{
	public Task<UserBlock> AddAsync(UserBlock domain)
	{
		throw new NotImplementedException();
	}

	public Task<OperationResult> AddRangeAsync(IEnumerable<UserBlock> domains)
	{
		throw new NotImplementedException();
	}

	public async Task<OperationResult> BlockOrUnblockAsync(UserBlock domain)
	{
		if (domain.BlockedUserId == domain.BlockedByUserId)
		{
			throw new ParameterInvalidException();
		}

		if (!await _userRepository.CheckExistAsync(domain.BlockedUserId))
		{
			throw new ResourceNotFoundException($"User {domain.BlockedUserId} was not found");
		}

		if (!await _userRepository.CheckExistAsync(domain.BlockedByUserId))
		{
			throw new ResourceNotFoundException($"User {domain.BlockedByUserId} was not found");
		}

		var block = await _repository.GetBlockOrBlockedAsync(domain);
		if (block != null)
		{
			var resultDelete = await _repository.DeleteByIdAsync(block.Id);
			return new OperationResult(resultDelete > 0);
		}
		else
		{
			domain.InitCreation();
			var blockEntity = _mapper.Map<UserBlockEntity>(domain);
			await _repository.AddAsync(blockEntity);
			return new OperationResult(true);
		}
	}


	public Task<int> GetCountAsync(Expression<Func<UserBlock, bool>>? condition = null)
	{
		throw new NotImplementedException();
	}

	public async Task<List<UserBlock>> GetUserBlockAsync(Guid userId)
	{
		var result = await _repository.GetUserBlockAsync(userId);
		return result;
	}

	public Task<OperationResult> UpdateAsync(Guid id, UserBlock domain)
	{
		throw new NotImplementedException();
	}

	Task<IEnumerable<UserBlock>> IServiceBase<UserBlock>.GetAllAsync()
	{
		throw new NotImplementedException();
	}

	Task<UserBlock> IServiceBase<UserBlock>.GetByIdAsync(Guid id)
	{
		throw new NotImplementedException();
	}

	Task<IEnumerable<UserBlock>> IServiceBase<UserBlock>.GetByIdsAsync(List<Guid> ids, string fieldsName)
	{
		throw new NotImplementedException();
	}

	Task<QueryResult<UserBlock>> IServiceBase<UserBlock>.GetByPageAsync(QueryInfo queryInfo)
	{
		throw new NotImplementedException();
	}
}
