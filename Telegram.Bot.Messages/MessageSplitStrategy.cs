namespace Telegram.Bot.Messages;
/// <summary>
///     Message splitting strategy
/// </summary>
public enum MessageSplitStrategy {
    /// <summary>
    ///     By word
    /// </summary>
    Word,
    /// <summary>
    ///     By line
    /// </summary>
    Line,
    /// <summary>
    ///     By paragraph
    /// </summary>
    Paragraph
}