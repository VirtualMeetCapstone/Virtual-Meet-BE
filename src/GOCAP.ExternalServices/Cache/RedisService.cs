namespace GOCAP.ExternalServices;

public class RedisService(IConnectionMultiplexer redisConnection) : IRedisService
{
    private readonly IDatabase _db = redisConnection.GetDatabase(); 

    public async Task SetAsync(string key, string value) => await _db.StringSetAsync(key, value);
    public async Task<string?> GetAsync(string key) => await _db.StringGetAsync(key);
    public async Task DeleteAsync(string key) => await _db.KeyDeleteAsync(key);
    public async Task<bool> ExistsAsync(string key) => await _db.KeyExistsAsync(key);
    public async Task<long> IncrementAsync(string key, long value = 1) => await _db.StringIncrementAsync(key, value);
    public async Task<long> DecrementAsync(string key, long value = 1) => await _db.StringDecrementAsync(key, value);
    public async Task SetExpiryAsync(string key, TimeSpan expiry) => await _db.KeyExpireAsync(key, expiry);
    public async Task SetHashFieldAsync(string hashKey, string field, string value) => await _db.HashSetAsync(hashKey, field, value);
    public async Task<string?> GetHashFieldAsync(string hashKey, string field) => await _db.HashGetAsync(hashKey, field);
    public async Task<HashEntry[]> GetAllHashFieldsAsync(string hashKey) => await _db.HashGetAllAsync(hashKey);
    public async Task PushLeftAsync(string key, string value) => await _db.ListLeftPushAsync(key, value);
    public async Task PushRightAsync(string key, string value) => await _db.ListRightPushAsync(key, value);
    public async Task<string?> PopLeftAsync(string key) => await _db.ListLeftPopAsync(key);
    public async Task<string?> PopRightAsync(string key) => await _db.ListRightPopAsync(key);
    public async Task AddToSetAsync(string key, string value) => await _db.SetAddAsync(key, value);
    public async Task<bool> IsMemberOfSetAsync(string key, string value) => await _db.SetContainsAsync(key, value);
    public async Task AddToSortedSetAsync(string key, string value, double score) => await _db.SortedSetAddAsync(key, value, score);
    public async Task<SortedSetEntry[]> GetSortedSetByRankAsync(string key, int start, int stop) => await _db.SortedSetRangeByRankWithScoresAsync(key, start, stop);
}
