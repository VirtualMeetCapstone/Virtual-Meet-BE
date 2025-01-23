using StackExchange.Redis;

namespace GOCAP.Repository;

internal abstract class RedisRepository (IConnectionMultiplexer _redisConnection) : IRedisRepository
{
    // Set a string value in Redis with a specified key
    public async Task SetAsync(string key, string value)
    {
        var db = _redisConnection.GetDatabase();
        await db.StringSetAsync(key, value);
    }

    // Get a string value from Redis based on the key
    public async Task<string?> GetAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        return await db.StringGetAsync(key);
    }

    // Delete a key from Redis
    public async Task DeleteAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        await db.KeyDeleteAsync(key);
    }

    // Check if a key exists in Redis
    public async Task<bool> ExistsAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        return await db.KeyExistsAsync(key);
    }

    // Increment the value of a key (if the value is an integer)
    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        var db = _redisConnection.GetDatabase();
        return await db.StringIncrementAsync(key, value);
    }

    // Decrement the value of a key (if the value is an integer)
    public async Task<long> DecrementAsync(string key, long value = 1)
    {
        var db = _redisConnection.GetDatabase();
        return await db.StringDecrementAsync(key, value);
    }

    // Set expiration time for a key in Redis
    public async Task SetExpiryAsync(string key, TimeSpan expiry)
    {
        var db = _redisConnection.GetDatabase();
        await db.KeyExpireAsync(key, expiry);
    }

    // Hash operations (Set field in hash)
    public async Task SetHashFieldAsync(string hashKey, string field, string value)
    {
        var db = _redisConnection.GetDatabase();
        await db.HashSetAsync(hashKey, field, value);
    }

    // Get field value from hash
    public async Task<string?> GetHashFieldAsync(string hashKey, string field)
    {
        var db = _redisConnection.GetDatabase();
        return await db.HashGetAsync(hashKey, field);
    }

    // Get all fields in a hash
    public async Task<HashEntry[]> GetAllHashFieldsAsync(string hashKey)
    {
        var db = _redisConnection.GetDatabase();
        return await db.HashGetAllAsync(hashKey);
    }

    // List operations (push to left)
    public async Task PushLeftAsync(string key, string value)
    {
        var db = _redisConnection.GetDatabase();
        await db.ListLeftPushAsync(key, value);
    }

    // List operations (push to right)
    public async Task PushRightAsync(string key, string value)
    {
        var db = _redisConnection.GetDatabase();
        await db.ListRightPushAsync(key, value);
    }

    // List operations (pop from left)
    public async Task<string?> PopLeftAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        return await db.ListLeftPopAsync(key);
    }

    // List operations (pop from right)
    public async Task<string?> PopRightAsync(string key)
    {
        var db = _redisConnection.GetDatabase();
        return await db.ListRightPopAsync(key);
    }

    // Set operations (add to set)
    public async Task AddToSetAsync(string key, string value)
    {
        var db = _redisConnection.GetDatabase();
        await db.SetAddAsync(key, value);
    }

    // Check if member exists in a set
    public async Task<bool> IsMemberOfSetAsync(string key, string value)
    {
        var db = _redisConnection.GetDatabase();
        return await db.SetContainsAsync(key, value);
    }

    // Sorted Set operations (add to sorted set)
    public async Task AddToSortedSetAsync(string key, string value, double score)
    {
        var db = _redisConnection.GetDatabase();
        await db.SortedSetAddAsync(key, value, score);
    }

    // Get members from a sorted set by rank
    public async Task<SortedSetEntry[]> GetSortedSetByRankAsync(string key, int start, int stop)
    {
        var db = _redisConnection.GetDatabase();
        return await db.SortedSetRangeByRankWithScoresAsync(key, start, stop);
    }
}