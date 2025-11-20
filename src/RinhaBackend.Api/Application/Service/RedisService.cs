using RinhaBackend.Api.Application.Interface;
using RinhaBackend.Api.Domain.Entity;
using StackExchange.Redis;
using System.Text.Json;

namespace RinhaBackend.Api.Application.Service;

public class RedisService : IRedisService
{
    private readonly IDatabase _database;
    private const string KEY_PAYMENTS_PROCESSED = "payments";

    public RedisService(IConnectionMultiplexer connectionMultiplexer)
        => _database = connectionMultiplexer.GetDatabase();

    public async Task AddPaymentAsync(Payment value)
    {
        var json = JsonSerializer.Serialize(value);
        double score = value.RequestedAt.ToOADate();
        await _database.SortedSetAddAsync(KEY_PAYMENTS_PROCESSED, json, score);
    }

    public async Task<List<Payment>?> GetPaymentsAsync(DateTime? start = null, DateTime? end = null)
    {
        double minScore = start?.ToOADate() ?? double.NegativeInfinity;
        double maxScore = end?.ToOADate() ?? double.PositiveInfinity;

        var results = await _database.SortedSetRangeByScoreAsync(KEY_PAYMENTS_PROCESSED, minScore, maxScore);
        return results.Select(x => JsonSerializer.Deserialize<Payment>(x!)!)
            .ToList();
    }
}