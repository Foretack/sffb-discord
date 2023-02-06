using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using SFFBot.Config;

namespace SFFBot;
internal sealed class Bot
{
    public static Dictionary<ulong, DiscordMember> MemberCache { get; } = new();

    public DiscordClient Client { get; init; }
    public IAppConfig Config { get; init; }
    public Redis Redis { get; init; }

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

    private Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
    {
        Log.Verbose("Checking message from {user}: {message}", e.Author.Username, e.Message.Content);
        return FilterHandler.CheckMessage(e);
    }
}
