namespace Telegram.Bot.Messages;
/// <summary>
///     Extension methods for <see cref="MessageBuilder"/>
/// </summary>
public static class MessageBuilderExtension {
    /// <summary>
    ///     Builds a message slice and splits it into multiple slices
    /// </summary>
    /// <param name="builder">Message builder</param>
    /// <param name="splitStrategy">The strategy used to split the message</param>
    /// <param name="contentType">The type of message content</param>
    /// <returns>An array of resulting message slices</returns>
    public static MessageSlice[] Build(this MessageBuilder builder, 
        MessageSplitStrategy splitStrategy, MessageContentType contentType
    ) {
        var result = builder.Build();
        switch (contentType) {
            case MessageContentType.TextOnly: 
                return MessageSliceHelper.Split(result, splitStrategy, 4096);
            case MessageContentType.TextWithAttachments: 
                return MessageSliceHelper.Split(result, splitStrategy, 1024, 4096);
            default:
                throw new MessageSplitException($"Failed to process message with content type {contentType}");
        }
    }
}