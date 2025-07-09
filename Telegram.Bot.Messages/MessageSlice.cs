using Telegram.Bot.Types;

namespace Telegram.Bot.Messages;
/// <summary>
///     Message slice
/// </summary>
/// <param name="Content">Text</param>
/// <param name="Entities">Styled segments</param>
public record MessageSlice(string Content, MessageEntity[] Entities) {
    /// <summary>
    ///     Size of message slice
    /// </summary>
    public int Size => Content.Length;
}