﻿namespace GOCAP.Services.Intention;

public interface IServiceBase<T>
{
    Task<T> AddAsync(T domain);
    Task<OperationResult> AddRangeAsync(IEnumerable<T> domains);
    Task<OperationResult> UpdateAsync(Guid id, T domain);
    Task<OperationResult> DeleteByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> GetCountAsync(Expression<Func<T, bool>>? condition = null);
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids, string fieldsName);
    Task<QueryResult<T>> GetByPageAsync(QueryInfo queryInfo);
    Task<OperationResult> CheckExistAsync(Guid id, string name);
    Task<OperationResult> CheckExistAsync(Guid id);
}
