using CachingFramework.Redis.Contracts.Providers;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SFFBot.Filters;
using SFFBot.Interfaces;
using SFFBot.Models;
using SFFBot.Utils;

namespace SFFBot;
internal sealed class FilterHandler
{
    private static readonly Dictionary<string, IFilter> _filters = new();
    private static ICacheProviderAsync _cache = default!;

    public FilterHandler(ICacheProviderAsync cache)
    {
        _cache = cache;

        AddFilter(new ServerInvitesFilter(true));
    }

    public static async Task CheckMessage(MessageCreateEventArgs message)
    {
        foreach ((_, IFilter filter) in _filters)
        {
            if (await filter.CheckContent(message))
            {
                Log.Information("Message {message} violates filter {fiilter}", message.Message.Content, filter.Name);

                DiscordMember member;
                if (Bot.MemberCache.ContainsKey(message.Author.Id))
                {
                    member = Bot.MemberCache[message.Author.Id];
                    Log.Debug("Fetched {user} from local cache.");
                }
                else
                {
                    Log.Debug("User {user} doesn't exist in {cache}. Adding entry", message.Author.Username, nameof(Bot.MemberCache));
                    member = await message.Guild.GetMemberAsync(message.Author.Id);
                    Bot.MemberCache.Add(message.Author.Id, member);
                }

                ActionType action = await filter.Punish(member);
                if (action > ActionType.Kick) await member.DeleteMessages();

                var dict = await _cache.FetchObjectAsync(
                    $"discord:{member.Guild.Id}:offenders",
                    () => Task.FromResult(new Dictionary<ulong, string>())
                );
                dict.Add(member.Id, action.ToString());
                await _cache.SetObjectAsync($"discord:{member.Guild.Id}:offenders", dict);

                break;
            }
        }
    }

    private static void AddFilter(IFilter chatFilter)
    {
        _filters.Add(chatFilter.Name, chatFilter);
        Log.Debug("Chat filter added: {filter}", chatFilter.Name);
    }
}
