namespace SFFBot.Config;
public interface IAppConfig
{
    public string Token { get; }
    public ulong WhitelistedRole { get; }

    public string TwitchToken { get; }
    public string TwitchClientId { get; }
}
