namespace Telegram.Bot.Messages;
/// <summary>
///     Exception from message builder domain
/// </summary>
public class MessageSplitException : Exception {
    /// <summary>
    ///     Constructor of exception
    /// </summary>
    /// <param name="message">Message</param>
    public MessageSplitException(string message) : base(message) {}
}