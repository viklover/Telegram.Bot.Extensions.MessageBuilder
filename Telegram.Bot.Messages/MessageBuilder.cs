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
    public MessageBuilder Append(char symbol) {
        _builder.Append(symbol);
        _currentIndex++;
        return this;
    }
    /// <summary>
    ///     Appends a string
    /// </summary>
    /// <param name="text">String</param>
    public MessageBuilder Append(string text) {
        _builder.Append(text);
        _currentIndex += text.Length;
        return this;
    }
    /// <summary>
    ///     Appends a content from another message builder
    /// </summary>
    /// <param name="message">Message builder</param>
    public MessageBuilder Append(MessageBuilder message) {
        foreach (var entity in message._entities) {
            entity.Offset = _currentIndex + entity.Offset;
            _entities.AddLast(entity);
        }
        Append(message._builder.ToString());
        return this;
    }
    /// <summary>
    ///     Appends a indent
    /// </summary>
    public MessageBuilder AppendLine() => Append('\n');
    /// <summary>
    ///     Appends a string with indent
    /// </summary>
    /// <param name="text">String</param>
    public MessageBuilder AppendLine(string text) {
        Append(text);
        Append("\n");
        return this;
    }
    /// <summary>
    ///     Appends a preformatted text block wrapped in <pre> tags
    /// </summary>
    /// <param name="text">The text to format as preformatted</param>
    /// <param name="lang">Language for text block</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendPreformatted(string text, string? lang = null, bool nl = false) => AppendEntity(text, MessageEntityType.Pre, nl, language: lang);
    /// <summary>
    ///     Appends a preformatted text block wrapped in <pre> tags with line break
    /// </summary>
    /// <param name="text">The text to format as preformatted</param>
    /// <param name="lang">Language for text block</param>
    public MessageBuilder AppendPreformattedLine(string text, string? lang = null) => AppendPreformatted(text, nl: true, lang: lang);
    /// <summary>
    ///      Appends a bold text block wrapped in <b> tags
    /// </summary>
    /// <param name="text">The text to format as bold</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendBold(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Bold, nl);
    /// <summary>
    ///      Appends a bold text block wrapped in <b> tags with line break
    /// </summary>
    /// <param name="text">The text to format as bold</param>
    public MessageBuilder AppendBoldLine(string text) => AppendBold(text, true);
    /// <summary>
    ///     Appends a italic text block wrapepd in <i> tags
    /// </summary>
    /// <param name="text">The text to format as italic</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendItalic(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Italic, nl);
    /// <summary>
    ///     Appends a italic text block wrapepd in <i> tags with line break
    /// </summary>
    /// <param name="text">The text to format as italic</param>
    public MessageBuilder AppendItalicLine(string text) => AppendItalic(text, true);
    /// <summary>
    ///     Appends a underlined text block wrapped in <u> tags
    /// </summary>
    /// <param name="text">The text to format as underlined</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendUnderlined(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Underline, nl);
    /// <summary>
    ///     Appends a underlined text block wrapped in <u> tags with line break
    /// </summary>
    /// <param name="text">The text to format as underlined</param>
    public MessageBuilder AppendUnderlinedLine(string text) => AppendUnderlined(text, true);
    /// <summary>
    ///     Appends a strikethrough text block wrapped in <s> tags
    /// </summary>
    /// <param name="text">The text to format as underlined</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendStrikethrough(string text, bool nl = false) => AppendEntity(text, MessageEntityType.Strikethrough, nl);
    /// <summary>
    ///     Appends a strikethrough text block wrapped in <s> tags with line break
    /// </summary>
    /// <param name="text">The text to format as underlined</param>
    public MessageBuilder AppendStrikethroughLine(string text) => AppendStrikethrough(text, true);
    /// <summary>
    ///     Appends a code text block wrapped in <code> tags
    /// </summary>
    /// <param name="code">The text to format as code</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendCode(string code, bool nl = false) => AppendEntity(code, MessageEntityType.Code, nl);
    /// <summary>
    ///     Appends a code text block wrapped in <code> tags with line break
    /// </summary>
    /// <param name="code">The text to format as code</param>
    public MessageBuilder AppendCodeLine(string code, bool nl = false) => AppendCode(code, true);
    /// <summary>
    ///    Appends a link text block wrapped in <a> tags
    /// </summary>
    /// <param name="label">Label</param>
    /// <param name="uri">URI link</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendLink(string label, Uri uri, bool nl = false) => AppendEntity(label, MessageEntityType.TextLink, nl, url: uri.ToString());
    /// <summary>
    ///    Appends a link text block wrapped in <a> tags with line break
    /// </summary>
    /// <param name="label">Label</param>
    /// <param name="uri">URI link</param>
    public MessageBuilder AppendLinkLine(string label, Uri uri) => AppendLink(label, uri, true);
    /// <summary>
    ///     Appends a field with value
    /// </summary>
    /// <param name="name">Field name</param>
    /// <param name="value">Field value</param>
    /// <param name="boldTitle">If true, field name wiil be bold</param>
    public MessageBuilder AppendField(string name, string value, bool nl = true, bool boldTitle = false) {
        if (boldTitle) {
            AppendBold(name, false);
        } else {
            Append(name);
        }
        Append(FieldSeparatorSymbol);
        Append(value);
        if (nl) {
            AppendLine();
        }
        return this;
    }
    /// <summary>
    ///     Appends a field with value
    /// </summary>
    /// <param name="name">Field name</param>
    /// <param name="printer">Field value printer</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <param name="boldTitle">If true, field name will be bold</param>
    public MessageBuilder AppendField(string name, Action<MessageBuilder> printer, bool nl = true, bool boldTitle = false) {
        var builder = new MessageBuilder {
            FieldSeparatorSymbol = FieldSeparatorSymbol
        };
        printer(builder);
        if (builder.Length == 0) {
            return this;
        }
        if (boldTitle) {
            AppendBold(name, false);
        } else {
            Append(name);
        }
        Append(FieldSeparatorSymbol);
        Append(builder);
        if (nl) {
            AppendLine();
        }
        return this;
    }
    /// <summary>
    ///     Appends a paragraph
    /// </summary>
    /// <param name="printer">Paragraph content printer</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    public MessageBuilder AppendParagraph(Action<MessageBuilder> printer, bool nl = true) => AppendParagraph(null, printer, nl);
    /// <summary>
    ///     Appends a paragraph
    /// </summary>
    /// <param name="title">Paragraph's title</param>
    /// <param name="printer">Paragraph content printer</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <param name="boldTitle">Bold title</param>
    public MessageBuilder AppendParagraph(string? title, Action<MessageBuilder> printer, bool nl = true, bool boldTitle = false) {
        var paragraph = new MessageBuilder {
            FieldSeparatorSymbol = FieldSeparatorSymbol
        };
        if (title != null) {
            if (boldTitle) {
                paragraph.AppendBoldLine(title);
            } else {
                paragraph.AppendLine(title);
            }
        }
        var initialLengthWithTitle = paragraph.Length;
        printer(paragraph);
        if (paragraph.Length == initialLengthWithTitle) {
            return this;
        }
        Append(paragraph);
        if (nl) {
            AppendLine();
        }
        return this;
    }
    /// <summary>
    ///     Appends a message entity
    /// </summary>
    /// <param name="text">Text</param>
    /// <param name="type">Message entity type</param>
    /// <param name="nl">If true, appends a newline at the end of the string</param>
    /// <param name="url">URL (for `URL` message entity type)</param>
    /// <param name="userId">User identifier (for 'TextMention' message entity type)</param>
    /// <param name="language">Language (for `Pre` message entity type)</param>
    public MessageBuilder AppendEntity(string text, MessageEntityType type, bool nl, 
        string? url = null, long? userId = null, string? language = null
    ) {
        var entity = new MessageEntity {
            Offset = _currentIndex,
            Length = text.Length,
            Type = type,
            Url = url,
            User = userId != null ? new() { Id = userId.Value } : null,
            Language = language
        };
        _entities.AddLast(entity);
        Append(text);
        if (nl) {
            Append('\n');
        }
        return this;
    }
    /// <summary>
    ///     Trims trailing whitespace from end of message builder content
    /// </summary>
    public MessageBuilder TrimEnd() {
        if (_builder.Length <= 1) {
            return this;
        }
        var textChars = new char[_builder.Length];
        _builder.CopyTo(0, textChars, 0, _builder.Length);
        var text = new string(textChars);
        if (text[^1] != '\n' || text[^2] != '\n') {
            return this;
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
        return this;
    }
    /// <summary>
    ///     Builds a message slice containing the message content and its segments
    /// </summary>
    /// <returns>Result <see cref="MessageSlice"/></returns>
    public MessageSlice Build() => new(_builder.ToString(), _entities.ToArray());
}