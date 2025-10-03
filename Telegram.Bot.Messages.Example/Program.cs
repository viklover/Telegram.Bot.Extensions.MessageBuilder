using Telegram.Bot;
using Telegram.Bot.Messages;

var builder = new MessageBuilder();
builder.Append("Hello");
builder.AppendBoldLine(", my dear user!");
builder.AppendLine();
builder.AppendParagraph("🧙 Styled entities", printer => {
    printer.AppendField("1", value => value.AppendBold("Bold text"));
    printer.AppendField("2", value => value.AppendItalic("Italic text"));
    printer.AppendField("3", value => value.AppendUnderlined("Underlined text"));
    printer.AppendField("4", value => value.AppendStrikethrough("Strikethrough text"));
    printer.AppendField("5", value => value.AppendCode("Code text"));
    printer.AppendField("6", value => value.Append("Preformatted text").AppendPreformatted("Hey"));
});
builder.AppendLine("C#");
builder.AppendPreformattedLine("var result = 4 + 4;", "csharp");
builder.AppendLine();
builder.AppendLink("Click to link!", new Uri("https://google.com"));
builder.AppendLine();

var userId = 1;

var message = builder.Build();
var bot = new TelegramBotClient("TOKEN", cancellationToken: CancellationToken.None);
await bot.SendMessage(userId, message.Content, entities: message.Entities);

// ... or if your message may exceed size limits, just split it into slices
var slices = builder.Build(MessageSplitStrategy.Paragraph, MessageContentType.TextWithAttachments);
foreach (var slice in slices) {
    await bot.SendMessage(userId, slice.Content, entities: slice.Entities);
}

