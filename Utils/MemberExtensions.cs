using DSharpPlus.Entities;

namespace SFFBot.Utils;
internal static class MemberExtensions
{
    public static async Task DeleteMessages(this DiscordMember member)
    {
        foreach (var e in Bot.MessageCache.Where(x => x.Author.Id == member.Id))
        {
            if ((DateTime.Now - e.Message.CreationTimestamp).TotalDays >= 10) continue;
            await e.Message.DeleteAsync();
            await Task.Delay(250);
        }
    }
}
