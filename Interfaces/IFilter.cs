using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SFFBot.Models;

namespace SFFBot.Interfaces;
public interface IFilter
{
    public string Name { get; }
    public bool Enabled { get; }
    /// <summary>
    /// Checks whether the content of a message violates filters or not
    /// </summary>
    /// <param name="message">The message to check</param>
    /// <returns>bool: violation</returns>
    public Task<bool> CheckContent(MessageCreateEventArgs message);
    public Task<ActionType> Punish(DiscordMember memeber);

    public void Enable();
    public void Disable();
}
