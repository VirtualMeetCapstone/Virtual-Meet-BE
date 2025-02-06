namespace GOCAP.Repository.Intention;

public interface IRepositoryBase<T>
{
    /// <summary>
    /// Add a record to the database.
    /// </summary>
    /// <param name="domain">domain model</param>
    Task<T> AddAsync(T domain);

    /// <summary>
    /// Add multiple records to the database.
    /// </summary>
    /// <param name="domains">domain model</param>
    /// <returns>True:succeed, False:failed.</returns>
    Task<bool> AddRangeAsync(IEnumerable<T> domains);

    /// <summary>
    /// Get all records in database, take care of the data size
    /// </summary>
    /// <returns>domain model list</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Retrieves the total count of records in the database with condition.
    /// </summary>
    /// <returns>number count</returns>
    Task<int> GetCountAsync(Expression<Func<T, bool>>? condition);
        
    /// <summary>
    /// Get first record which matches the id
    /// </summary>
    /// <param name="id">domain model id</param>
    /// <returns>domain model</returns>
    Task<T> GetByIdAsync(Guid id);

    /// <summary>
    /// Get records list by Ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    Task<IEnumerable<T>> GetByIdsAsync(List<Guid> ids, string fieldsName);

    /// <summary>
    /// Get the summary of target domain model, only support search currently
    /// </summary>
    /// <param name="search">search text</param>
    /// <param name="sortBy">column name order by</param>
    /// <param name="sortOrder">order type</param>
    /// <param name="top">select top number</param>
    /// <param name="skip">skip number</param>
    /// <returns></returns>
    Task<QueryResult<T>> GetByPageAsync(QueryInfo queryInfo);

    /// <summary>
    /// Update a record in database.
    /// </summary>
    /// <param name="domain">domain model</param>
    /// <returns>True:succeed, False:failed.</returns>
    Task<bool> UpdateAsync(Guid id, T domain);

    /// <summary>
    /// Delete a record from the database.
    /// </summary>
    /// <param name="id">domain model id</param>
    /// <returns>True:succeed, False:failed.</returns>
    Task<int> DeleteByIdAsync(Guid id);

    /// <summary>
    /// Check the record exist or not by id and name
    /// </summary>
    /// <param name="id">domain model id</param>
    /// <param name="name">domain model name</param>
    /// <returns>True: if there is no record equals by name but id not, otherwise False</returns>
    Task<bool> CheckExistAsync(Guid id, string name);

    /// <summary>
    /// Check the record exist or not by id 
    /// </summary>
    /// <param name="id">domain model id</param>
    /// <returns>True: if there is no record equals by name but id not, otherwise False</returns>
    Task<bool> CheckExistAsync(Guid id);
}