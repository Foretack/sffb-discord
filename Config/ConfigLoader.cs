using Config.Net;

namespace SFFBot.Config;
internal static class ConfigLoader
{
    public static IAppConfig BotConfig => _botConfig;
    public static IRedisConfig RedisConfig => _redisConfig;

    private static IAppConfig _botConfig = default!;
    private static IRedisConfig _redisConfig = default!;


    public static void Initialize()
    {
        IAppConfig config = new ConfigurationBuilder<IAppConfig>().UseYamlFile("bot_config.yml").Build();
        _botConfig = config;
        IRedisConfig redisConfig = new ConfigurationBuilder<IRedisConfig>().UseYamlFile("redis_config.yml").Build();
        _redisConfig = redisConfig;
        Log.Information("Configs initialized");

        _ = new Timer(_ =>
        {
            _botConfig = new ConfigurationBuilder<IAppConfig>().UseYamlFile("bot_config.yml").Build();
            redisConfig = new ConfigurationBuilder<IRedisConfig>().UseYamlFile("redis_config.yml").Build();
            Log.Debug("Reloaded configs");
        }, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30));
    }
}
