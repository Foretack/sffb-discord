using CachingFramework.Redis;
using CachingFramework.Redis.Contracts.Providers;
using CachingFramework.Redis.Serializers;
using SFFBot.Config;

namespace SFFBot;
internal sealed class Redis
{
    public ICacheProviderAsync Cache { get; init; }
    public IPubSubProviderAsync PubSub { get; init; }

    private static RedisContext _context = default!;

    public Redis()
    {
        IRedisConfig config = ConfigLoader.RedisConfig;
        _context = new RedisContext($"{config.Host},password={config.Pass}", new JsonSerializer());
        Cache = _context.Cache;
        PubSub = _context.PubSub;
        Log.Information("Connected to Redis ({host})", config.Host);
    }
}
