using StackExchange.Redis;

namespace GOCAP.Services;

public class RedisService (IRedisRepository _repository): IRedisService
{
    // Set a string value in Redis with a specified key
    public async Task SetAsync(string key, string value)
    {
        await _repository.SetAsync(key, value);
    }

    // Get a string value from Redis based on the key
    public async Task<string?> GetAsync(string key)
    {
        return await _repository.GetAsync(key);
    }

    // Delete a key from Redis
    public async Task DeleteAsync(string key)
    {
        await _repository.DeleteAsync(key);
    }

    // Check if a key exists in Redis
    public async Task<bool> ExistsAsync(string key)
    {
        return await _repository.ExistsAsync(key);
    }

    // Increment the value of a key (if the value is an integer)
    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        return await _repository.IncrementAsync(key, value);
    }

    // Decrement the value of a key (if the value is an integer)
    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        return await _repository.DecrementAsync(key, (long)value);
    }

    // Set expiration time for a key in Redis
    public async Task SetExpiryAsync(string key, TimeSpan expiry)
    {
        await _repository.SetExpiryAsync(key, expiry);
    }

    // Hash operations (Set field in hash)
    public async Task SetHashFieldAsync(string hashKey, string field, string value)
    {
        await _repository.SetHashFieldAsync(hashKey, field, value);
    }

    // Get field value from hash
    public async Task<string?> GetHashFieldAsync(string hashKey, string field)
    {
        return await _repository.GetHashFieldAsync(hashKey, (string)field);
    }

    // Get all fields in a hash
    public async Task<HashEntry[]> GetAllHashFieldsAsync(string hashKey)
    {
        return await _repository.GetAllHashFieldsAsync(hashKey);
    }

    // List operations (push to left)
    public async Task PushLeftAsync(string key, string value)
    {
        await _repository.PushLeftAsync(key,value);
    }

    // List operations (push to right)
    public async Task PushRightAsync(string key, string value)
    {
        await _repository.PushRightAsync(key, value);
    }

    // List operations (pop from left)
    public async Task<string?> PopLeftAsync(string key)
    {
        return await _repository.PopLeftAsync(key);
    }

    // List operations (pop from right)
    public async Task<string?> PopRightAsync(string key)
    {
        
        return await _repository.PopRightAsync(key);
    }

    // Set operations (add to set)
    public async Task AddToSetAsync(string key, string value)
    {
        await _repository.AddToSetAsync(key, value);
    }

    // Check if member exists in a set
    public async Task<bool> IsMemberOfSetAsync(string key, string value)
    {
        return await _repository.IsMemberOfSetAsync(key, value);
    }

    // Sorted Set operations (add to sorted set)
    public async Task AddToSortedSetAsync(string key, string value, double score)
    {
        await _repository.AddToSortedSetAsync(key, value, score);
    }

    // Get members from a sorted set by rank
    public async Task<SortedSetEntry[]> GetSortedSetByRankAsync(string key, int start, int stop)
    {
        return await _repository.GetSortedSetByRankAsync(key, start, stop);    
    }
}
