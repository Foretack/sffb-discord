using System.Buffers;
using DSharpPlus.Entities;

namespace SFFBot.Utils;
internal static class MemberExtensions
{
    public static async Task DeleteMessages(this DiscordMember member)
    {
        const int maxAmount = 128;
        int index = 0;
        DiscordChannel[] channels = member.Guild.Channels.Select(x => x.Value).ToArray();
        ulong userid = member.Id;

        foreach (var channel in channels)
        {
            var userMessages = ArrayPool<DiscordMessage>.Shared.Rent(maxAmount);

            var messageList = await channel.GetMessagesAsync(maxAmount);
            foreach (var message in messageList.Where(x => x.Author.Id == userid))
            {
                if (index >= maxAmount - 1) break;
                userMessages[index++] = message;
            }
            await channel.DeleteMessagesAsync(userMessages);

            ArrayPool<DiscordMessage>.Shared.Return(userMessages);
        }
    }
}
