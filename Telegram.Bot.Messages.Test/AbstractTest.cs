using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Messages.Test;
/// <summary>
///     Abstract class for all tests
/// </summary>
public abstract class AbstractTest {
    /// <summary>
    ///     Random number generator
    /// </summary>
    public static readonly Random Random = new(); 
    /// <summary>
    ///     Generate a random integer in borders
    /// </summary>
    /// <param name="min">Lower border</param>
    /// <param name="max">Upper border</param>
    /// <returns>Result integer</returns>
    protected static int GenerateRandomInt(int min, int max) => Random.Next(min, max);
    /// <summary>
    ///     Generate a random string
    /// </summary>
    protected static string GenerateRandomString() => Guid.NewGuid().ToString();
    /// <summary>
    ///     Generate a random string with specific length
    /// </summary>
    /// <param name="length">Length</param>
    /// <returns>Result string</returns>
    protected static string GenerateRandomString(int length) {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new StringBuilder(length);
        for (var i = 0; i < length; i++) {
            var nextChar = chars[Random.Next(chars.Length)];
            result.Append(nextChar);
        }
        return result.ToString();
    }
    /// <summary>
    ///     Generate a random <see cref="MessageEntityType"/>
    /// </summary>
    protected static MessageEntityType GenerateMessageEntityType() {
        return (MessageEntityType) GenerateRandomInt(1, 19);
    }
    /// <summary>
    ///     Generate a message slice with specific length of random content
    /// </summary>
    /// <param name="randomStringLength">Content length</param>
    /// <returns>Result message slice without styles</returns>
    protected static MessageSlice GenerateMessageSlice(int randomStringLength) {
        var message = GenerateRandomString(randomStringLength);
        return new MessageSlice(message, Array.Empty<MessageEntity>());
    }
    /// <summary>
    ///     Generate a message slice with specific content
    /// </summary>
    /// <param name="content">Slice content</param>
    /// <returns>Result message slice without styles</returns>
    protected static MessageSlice GenerateMessageSlice(string content) {
        return new MessageSlice(content, Array.Empty<MessageEntity>());
    }
}