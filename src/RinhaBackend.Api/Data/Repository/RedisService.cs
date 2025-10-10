using RinhaBackend.Api.Domain.Entity;
using RinhaBackend.Api.Domain.Interface;
using StackExchange.Redis;
using System.Text.Json;

namespace RinhaBackend.Api.Data.Repository;

public class RedisService
    : IRedisService
{
    private readonly IDatabase _database;
    private const string PAYMENT_KEY = "payment:";
    private const string PAYMENT_INDEX = "payments:by_time";

    public RedisService(IConnectionMultiplexer connectionMultiplexer) => _database = connectionMultiplexer.GetDatabase();

    public async Task<IEnumerable<Payment>> GetPaymentsAsync(DateTime? from, DateTime? to)
    {
        var minScore = from?.ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalSeconds ?? double.NegativeInfinity;
        var maxScore = to?.ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalSeconds ?? double.PositiveInfinity;

        var values = await _database.SortedSetRangeByScoreAsync(PAYMENT_INDEX, minScore, maxScore);
        var keys = values.Select(v => (RedisKey)$"{PAYMENT_KEY}{v}").ToArray();
        var results = await _database.StringGetAsync(keys);
        return results
            .Where(p => p.HasValue)
            .Select(p => JsonSerializer.Deserialize<Payment>(p!)!)
            .ToList();
    }

    public async Task SetPaymentAsync(Payment value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        var score = value.RequestedAt.ToUniversalTime().Subtract(DateTime.UnixEpoch).TotalSeconds;
        await _database.StringSetAsync($"{PAYMENT_KEY}{value.CorrelationId}", json, expiry);
        await _database.SortedSetAddAsync(PAYMENT_INDEX, value.CorrelationId.ToString(), score);
    }
}