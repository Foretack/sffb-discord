using System.Collections.Concurrent;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using SFFBot.Config;

namespace SFFBot;
internal sealed class Bot
{
    public static ConcurrentQueue<MessageCreateEventArgs> MessageCache { get; } = new();
    public static Dictionary<ulong, DiscordMember> MemberCache { get; } = new();

    public DiscordClient Client { get; init; }
    public IAppConfig Config { get; init; }
    public Redis Redis { get; init; }

    private const int MAX_MESSAGE_CACHE_SIZE = 1024;

    public Bot(Redis redisConnection)
    {
        Config = ConfigLoader.BotConfig;
        Redis = redisConnection;

        DiscordConfiguration discordConfig = new()
        {
            Token = Config.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged
                    | DiscordIntents.GuildBans
                    | DiscordIntents.GuildMessages
                    | DiscordIntents.MessageContents
                    | DiscordIntents.Guilds
                    | DiscordIntents.GuildPresences,
            LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger),
            LargeThreshold = 10000
        };

        Log.Information("Creating Discord client");
        Client = new(discordConfig);
        Task.Run(async () => await Client.ConnectAsync());

        Client.MessageCreated += MessageCreated;
    }

    private async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
    {
        Log.Verbose("Checking message from {user}: {message}", e.Author.Username, e.Message.Content);
        while (!CacheMessage(e)) await Task.Delay(1000);
        await FilterHandler.CheckMessage(e);
    }

    private static bool CacheMessage(MessageCreateEventArgs e)
    {
        if (MessageCache.Count < MAX_MESSAGE_CACHE_SIZE)
        {
            MessageCache.Enqueue(e);
            Log.Verbose("Caching message: {user}:{message}",
                e.Author.Username,
                e.Message.Content);
        }
        else if (!MessageCache.TryDequeue(out _))
        {
            Log.Warning("Cache dequeue failed.");
            return false;
        }
        else
        {
            Log.Verbose("Replacing cache message");
            Log.Verbose("Caching message: {user}:{message}",
                e.Author.Username,
                e.Message.Content);
            MessageCache.Enqueue(e);
        }
        return true;
    }
}
