namespace GOCAP.ExternalServices;

public interface IRedisService
{
    // Set a string value in Redis with a specified key
    Task SetAsync(string key, string value);

    // Get a string value from Redis based on the key
    Task<string?> GetAsync(string key);

    // Delete a key from Redis
    Task DeleteAsync(string key);

    // Check if a key exists in Redis
    Task<bool> ExistsAsync(string key);

    // Increment the value of a key (if the value is an integer)
    Task<long> IncrementAsync(string key, long value = 1);

    // Decrement the value of a key (if the value is an integer)
    Task<long> DecrementAsync(string key, long value = 1);

    // Set expiration time for a key in Redis
    Task SetExpiryAsync(string key, TimeSpan expiry);

    // Hash operations (Set field in hash)
    Task SetHashFieldAsync(string hashKey, string field, string value);

    // Get field value from hash
    Task<string?> GetHashFieldAsync(string hashKey, string field);

    // Get all fields in a hash
    Task<HashEntry[]> GetAllHashFieldsAsync(string hashKey);

    // List operations (push to left)
    Task PushLeftAsync(string key, string value);

    // List operations (push to right)
    Task PushRightAsync(string key, string value);

    // List operations (pop from left)
    Task<string?> PopLeftAsync(string key);

    // List operations (pop from right)
    Task<string?> PopRightAsync(string key);

    // Set operations (add to set)
    Task AddToSetAsync(string key, string value);

    // Check if member exists in a set
    Task<bool> IsMemberOfSetAsync(string key, string value);

    // Sorted Set operations (add to sorted set)
    Task AddToSortedSetAsync(string key, string value, double score);

    // Get members from a sorted set by rank
    Task<SortedSetEntry[]> GetSortedSetByRankAsync(string key, int start, int stop);
}
