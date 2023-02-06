using System.Text.RegularExpressions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SFFBot.Config;
using SFFBot.Interfaces;
using SFFBot.Models;

namespace SFFBot.Filters;
internal class ServerInvitesFilter : IFilter
{
    public string Name => this.GetType().Name;
    public bool Enabled { get; private set; }

    private readonly Regex _inviteRegex = new(@"\b(https?:[/\\][/\\])?discord\.gg/[a-zA-Z0-9]{4,}\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(25));

    public ServerInvitesFilter(bool enabled)
    {
        if (enabled) Enable();
    }

    public async Task<bool> CheckContent(MessageCreateEventArgs message)
    {
        if (!Enabled || message.Author.IsBot) return false;

        DiscordMember member;
        if (Bot.MemberCache.ContainsKey(message.Author.Id)) member = Bot.MemberCache[message.Author.Id];
        else
        {
            member = await message.Guild.GetMemberAsync(message.Author.Id);
            Bot.MemberCache.Add(message.Author.Id, member);
        }

        if (member.Roles?.Select(x => x.Id)?.Any(x => ConfigLoader.BotConfig.WhitelistedRole == x) ?? false)
            return false;
        if (!_inviteRegex.IsMatch(message.Message.Content)) return false;
        return true;
    }

    public async Task<ActionType> Punish(DiscordMember member)
    {
        await member.TimeoutAsync(DateTime.Now.AddDays(7), "Posted a Discord invite link");
        return ActionType.Timeout;
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }
}
