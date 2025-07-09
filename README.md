# Message builder for .NET Client for Telegram Bot 
Simple message builder for [Telegram.Bot .NET client](https://github.com/TelegramBots/Telegram.Bot) helps create formatted text with styled entities (bold, italic, code, etc.) 
and split long messages to fit Telegram limits.

<img src="./.github/pictures/img.png" width="300">

## 🚀 Quick start
Installation:
```csharp
dotnet add package Telegram.Bots.Messages
```
Example usage:
```csharp
var builder = new MessageBuilder();
builder.Append("Hello");
builder.AppendBoldLine(", my dear user!");
builder.AppendLine();
builder.AppendParagraph("🧙 Styled entities", (MessageBuilder printer) => {
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

var message = builder.Build();

// send the message with formatted entities
await bot.SendMessage(chatId, message.Content, entities: message.Entities);

// for long messages: split by paragraphs (handles text+attachments)
var slices = builder.Build(MessageSplitStrategy.Paragraph, MessageContentType.TextWithAttachments);
foreach (var message in slices) {
    await bot.SendMessage(chatId, message.Content, entities: message.Entities);
}
```

## 📚 Basics
### Supported message entities
Telegram has [documentation about all styled entities](https://core.telegram.org/api/entities).

| Message entity    | State           |
|-------------------|-----------------|
| Bold              | ✅ Supported     |
| Italic            | ✅ Supported     |
| Code              | ✅ Supported     |
| Strike            | ✅ Supported     |
| Underline         | ✅ Supported     |
| Preformatted text | ✅ Supported     |
| Mention user name | ⏳ Not supported |
| Custom emoji      | ⏳ Not supported |

### Content splitting strategy
| Split strategy | Demonstration |
|----------------|---------------|
| By word        |           |
| By line        |           |
| By paragraph   |           |


### Message size limits
Telegram has limits to messages content size depends on presence attachments in your message:

| Message Type       | Character Limit  |
|--------------------|------------------|
| With attachments   | 1024             |
| Text-only messages | 4096             |

So, for correct splitting message you should specify `MessageContentType`:
```csharp
var slices = builder.Build(
    MessageSplitStrategy.Paragraph, 
    MessageContentType.TextWithAttachments
);
```

### Builder methods signatures
```csharp
MessageBuilder Append(char symbol)
MessageBuilder Append(MessageBuilder builder)
MessageBuilder AppendLine()
MessageBuilder AppendLine(string text)

MessageBuilder AppendBold(string text, bool nl = false)
MessageBuilder AppendBoldLine(string text)

MessageBuilder AppendItalic(string text, bool nl = false)
MessageBuilder AppendItalicLine(string text)

MessageBuilder AppendUnderlined(string text, bool nl = false)
MessageBuilder AppendUnderlinedLine(string text)

MessageBuilder AppendStrikethrough(string text, bool nl = false)
MessageBuilder AppendStrikethroughLine(string text)

MessageBuilder AppendPreformatted(string text, string? lang = null, bool nl = false)
MessageBuilder AppendPreformattedLine(string text, string? lang = null)

MessageBuilder AppendCode(string code, bool nl = false)
MessageBuilder AppendCodeLine(string code, bool nl = false)

MessageBuilder AppendLink(string label, Uri uri, bool nl = false) 
MessageBuilder AppendLinkLine(string label, Uri uri)

MessageBuilder AppendField(string name, string value, bool nl = true, bool boldTitle = false)
MessageBuilder AppendField(string name, Action<MessageBuilder> printer, bool nl = true, bool boldTitle = false)

MessageBuilder AppendParagraph(Action<MessageBuilder> printer, bool nl = true)
MessageBuilder AppendParagraph(string? title, Action<MessageBuilder> printer, bool nl = true, bool boldTitle = false)

MessageBuilder TrimEnd()
```


## 🛠️ Contribution
Pull requests are welcome!

