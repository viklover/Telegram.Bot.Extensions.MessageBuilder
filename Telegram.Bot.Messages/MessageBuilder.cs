using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Messages;
/// <summary>
///     Message builder
/// </summary>
public class MessageBuilder {
    private readonly StringBuilder _builder = new();
    private readonly LinkedList<MessageEntity> _entities = new();
    private int _currentIndex;
    /// <summary>
    ///     Current number of symbols in builder
    /// </summary>
    public int Length => _builder.Length;
    /// <summary>
    ///     Symbol for separating field name and value
    /// </summary>
    public string FieldSeparatorSymbol { get; init; } = ": ";
    /// <summary>
    ///     Constructor of message builder
    /// </summary>
    public MessageBuilder() {}
    /// <summary>
    ///     Constructor of message builder
    /// </summary>
    /// <param name="input">Beggining symbols for buffer</param>
    public MessageBuilder(string input) {
        Append(input);
    }
    /// <summary>
    ///     Appends a symbol
    /// </summary>
    /// <param name="symbol">Symbol</param>
    public void Append(char symbol) {
        _builder.Append(symbol);
        _currentIndex++;
    }
    /// <summary>
    ///     Appends a string
    /// </summary>
    /// <param name="text">String</param>
    public void Append(string text) {
        _builder.Append(text);
        _currentIndex += text.Length;
    }
    /// <summary>
    ///     Appends a content from another message builder
    /// </summary>
    /// <param name="message">Message builder</param>
    public void Append(MessageBuilder message) {
        foreach (var entity in message._entities) {
            entity.Offset = _currentIndex + entity.Offset;
            _entities.AddLast(entity);
        }
        Append(message._builder.ToString());
    }
    /// <summary>
    ///     Appends a indent
    /// </summary>
    public void AppendLine() => Append('\n');
    /// <summary>
    ///     Appends a string with indent
    /// </summary>
    /// <param name="text">String</param>
    public void AppendLine(string text) {
        Append(text);
        Append("\n");
    }
    /// <summary>
    ///     Appends a preformatted text block wrapped in <pre> tags
    /// </summary>
    /// <param name="text">The text to format as preformatted</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public void AppendPreformattedText(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Pre, nl);
    /// <summary>
    ///      Appends a bold text block wrapped in <b> tags
    /// </summary>
    /// <param name="text">The text to format as bold</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public void AppendBoldText(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Bold, nl);
    /// <summary>
    ///     Appends a italic text block wrapepd in <i> tags
    /// </summary>
    /// <param name="text">The text to format as italic</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public void AppendItalicText(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Italic, nl);
    /// <summary>
    ///     Appends a underlined text block wrapped in <u> tags
    /// </summary>
    /// <param name="text">The text to format as underlined</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public void AppendUnderlinedText(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Underline, nl);
    /// <summary>
    ///     Appends a code text block wrapped in <code> tags
    /// </summary>
    /// <param name="code">The text to format as code</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public void AppendCode(string code, bool nl = false) => AppendEntity(code, MessageEntityType.Code, nl);
    /// <summary>
    ///    Appends a link text block wrapped in <a> tags
    /// </summary>
    /// <param name="uri">URI link</param>
    /// <param name="str">Label</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public void AppendLink(Uri uri, string str, bool nl = false) => AppendEntity(str, MessageEntityType.Url, nl, url: uri.ToString());
    /// <summary>
    ///     Appends a field with value
    /// </summary>
    /// <param name="name">Field name</param>
    /// <param name="value">Field value</param>
    /// <param name="boldTitle">If true, field name wiil be bold</param>
    public void AppendField(string name, string value, bool nl = true, bool boldTitle = false) {
        if (boldTitle) {
            AppendBoldText(name, false);
        } else {
            Append(name);
        }
        Append(FieldSeparatorSymbol);
        Append(value);
        if (nl) {
            AppendLine();
        }
    }
    /// <summary>
    ///     Appends a field with value
    /// </summary>
    /// <param name="name">Field name</param>
    /// <param name="printer">Field value printer</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <param name="boldTitle">If true, field name will be bold</param>
    public void AppendField(string name, Action<MessageBuilder> printer, bool nl = true, bool boldTitle = false) {
        var builder = new MessageBuilder();
        printer(builder);
        if (builder.Length == 0) {
            return;
        }
        if (boldTitle) {
            AppendBoldText(name, false);
        } else {
            Append(name);
        }
        Append(FieldSeparatorSymbol);
        Append(builder);
        if (nl) {
            AppendLine();
        }
    }
    /// <summary>
    ///     Appends a paragraph
    /// </summary>
    /// <param name="printer">Paragraph content printer</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <returns>Indicates whether the paragraph result content is non-empty</returns>
    public bool AppendParagraph(Action<MessageBuilder> printer, bool nl = true) => AppendParagraph(null, printer, nl);
    /// <summary>
    ///     Appends a paragraph
    /// </summary>
    /// <param name="title">Paragraph's title</param>
    /// <param name="printer">Paragraph content printer</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <returns>Indicates whether the paragraph result content is non-empty</returns>
    public bool AppendParagraph(string? title, Action<MessageBuilder> printer, bool nl = true) {
        var paragraph = new MessageBuilder();
        if (title != null) {
            paragraph.AppendBoldText(title, true);
        }
        var initialLengthWithTitle = paragraph.Length;
        printer(paragraph);
        if (paragraph.Length == initialLengthWithTitle) {
            return false;
        }
        Append(paragraph);
        if (nl) {
            AppendLine();
        }
        return true;
    }
    /// <summary>
    ///     Appends a message entity
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="type">Message entity type</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <param name="url">URL (for `URL` message entity type)</param>
    public void AppendEntity(string text, MessageEntityType type, bool nl, string? url = null) {
        var entity = new MessageEntity {
            Offset = _currentIndex,
            Length = text.Length,
            Type = type,
            Url = url
        };
        _entities.AddLast(entity);
        Append(text);
        if (nl) {
            Append('\n');
        }
    }
    /// <summary>
    ///     Trims trailing whitespace from end of message builder content
    /// </summary>
    public void TrimEnd() {
        if (_builder.Length <= 1) {
            return;
        }
        var textChars = new char[_builder.Length];
        _builder.CopyTo(0, textChars, 0, _builder.Length);
        var text = new string(textChars);
        if (text[^1] != '\n' || text[^2] != '\n') {
            return;
        }
        var end = Length;
        var entities = new List<MessageEntity>(_entities);
        var entitiesIndex = _entities.Count - 1;
        while (end > 1 && text[end - 1] == '\n' && text[end - 1 - 1] == '\n') {
            var isWithinSection = false;
            while (entitiesIndex >= 0) {
                var entity = entities[entitiesIndex];
                var entityEnd = entity.Offset + entity.Length - 1;
                if (end - 1 >= entity.Offset && end <= entityEnd) {
                    isWithinSection = true;
                    break;
                }
                if (entityEnd < end) {
                    break;
                }
                entitiesIndex--;
            }
            if (isWithinSection) {
                break;
            }
            text = text.Remove(end - 1, 1);
            _builder.Remove(end - 1, 1);
            end--;
            _currentIndex--;
        }
    }
    /// <summary>
    ///     Builds a message slice containing the message content and its segments
    /// </summary>
    /// <returns>Result <see cref="MessageSlice"/></returns>
    public MessageSlice Build() => new(_builder.ToString(), _entities.ToArray());
}