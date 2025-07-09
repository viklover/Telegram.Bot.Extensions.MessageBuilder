using Telegram.Bot.Types;

namespace Telegram.Bot.Messages;
/// <summary>
///     Helper for <see cref="MessageSlice"/>
/// </summary>
public class MessageSliceHelper {
    /// <summary>
    ///     Раздробить слайс на несколько
    /// </summary>
    /// <param name="source">Исходный размер</param>
    /// <param name="strategy">Стратегия дробления слайса</param>
    /// <param name="firstSliceSize">Размер первого слайса</param>
    /// <param name="otherSlicesSize">Размер последующих слайсов</param>
    /// <returns>Массив с раздробленными слайсами</returns>
    public static MessageSlice[] Split(MessageSlice source, MessageSplitStrategy strategy, int firstSliceSize, int otherSlicesSize) {
        var slices = SubSlice(source, strategy, firstSliceSize);
        if (slices.Length is 1 or 0) {
            return slices;
        }
        var firstSlice = slices[0];
        var secondSlice = slices[1];
        var otherSlices = Split(secondSlice, strategy, otherSlicesSize);
        var result = new MessageSlice[1 + otherSlices.Length];
        result[0] = firstSlice;
        for (var i = 1; i < result.Length; ++i) {
            result[i] = otherSlices[i - 1];
        }
        return result;
    }
    /// <summary>
    ///     Раздробить слайс на равные слайсы по заданному размеру (по-возможности)
    /// </summary>
    /// <param name="source">Исходный слайс</param>
    /// <param name="strategy">Стратегия дробления слайса</param>
    /// <param name="sliceSize">Размер слайса</param>
    /// <returns>Массив с разделёнными слайсами</returns>
    public static MessageSlice[] Split(MessageSlice source, MessageSplitStrategy strategy, int sliceSize) {
        if (source.Size <= sliceSize) {
            var entities = source.Entities.ToArray();
            return new[] { source with { Entities = entities } };
        }
        var result = new LinkedList<MessageSlice>();
        var startIndex = 0;
        while (startIndex < source.Size) {
            var slice = Select(source, strategy, startIndex, sliceSize);
            if (slice == null) {
                break;
            } 
            result.AddLast(slice);
            startIndex += slice.Size;
        }
        return result.ToArray();
    }
    /// <summary>
    ///     Разделить слайс на два
    /// </summary>
    /// <param name="source">Исходный слайс</param>
    /// <param name="strategy">Стратегия дробления слайса</param>
    /// <param name="firstSliceSize">Размер первого слайса</param>
    /// <returns>Массив из одного или двух слайсов</returns>
    public static MessageSlice[] SubSlice(MessageSlice source, MessageSplitStrategy strategy, int firstSliceSize) {
        var text = source.Content;
        if (text.Length <= firstSliceSize) {
            return new[] { new MessageSlice(text, source.Entities.ToArray()) };
        }
        var firstSlice = Select(source, strategy, 0, firstSliceSize);
        if (firstSlice == null) {
            throw new Exception("Failed to select subSlice: source slice is too small");
        }
        var secondSlice = Select(source, strategy, firstSlice.Size);
        if (secondSlice == null) {
            return new[] { firstSlice };
        }
        return new[] { firstSlice, secondSlice };
    }
    /// <summary>
    ///     Вырезать слайс из другого слайса
    /// </summary>
    /// <param name="source">Исходный слайс</param>
    /// <param name="strategy">Стратегия дробления слайса</param>
    /// <param name="startIndex">Начальный индекс (по содержимому)</param>
    /// <returns>Вырезанный слайс</returns>
    public static MessageSlice? Select(MessageSlice source, MessageSplitStrategy strategy, int startIndex) {
        var sliceSize = source.Size - startIndex;
        return Select(source, strategy, startIndex, sliceSize);
    }
    /// <summary>
    ///     Вырезать слайс из другого слайса
    /// </summary>
    /// <param name="source">Исходный слайс</param>
    /// <param name="sliceSize">Размер выделения</param>
    /// <param name="strategy">Стратегия дробления слайса</param>
    /// <param name="startIndex">Начальный индекс (по содержимому)</param>
    /// <returns>Вырезанный слайс</returns>
    public static MessageSlice? Select(MessageSlice source, MessageSplitStrategy strategy, int startIndex, int sliceSize) {
        if (source.Size == 0 || startIndex >= source.Size || sliceSize is 0) {
            return null;
        }
        var text = source.Content;
        var sliceLength = Math.Min(sliceSize, text.Length - startIndex);
        if (startIndex + sliceLength < text.Length && !char.IsWhiteSpace(text[startIndex + sliceLength])) {
            if (strategy == MessageSplitStrategy.Word) {
                var lastSpaceIndex = text.LastIndexOf(' ', startIndex + sliceLength, sliceLength);
                if (lastSpaceIndex > startIndex) {
                    sliceLength = lastSpaceIndex - startIndex + 1;
                }
            } else if (strategy == MessageSplitStrategy.Line) {
                var lastSpaceIndex = text.LastIndexOf('\n', startIndex + sliceLength, sliceLength);
                if (lastSpaceIndex > startIndex) {
                    sliceLength = lastSpaceIndex - startIndex;
                }
            } else if (strategy == MessageSplitStrategy.Paragraph) {
                var lastSpaceIndex = text.LastIndexOf("\n\n", startIndex + sliceLength, sliceLength, StringComparison.InvariantCulture);
                if (lastSpaceIndex > startIndex) {
                    sliceLength = lastSpaceIndex - startIndex;
                }
            }
        }
        var content = text.Substring(startIndex, sliceLength);
        var entities = ResolveEntitiesForSlice(source.Entities, startIndex, sliceLength);
        return new MessageSlice(content, entities.ToArray());
    }
    /// <summary>
    ///     Раздробить стилевые отрезки текста
    /// </summary>
    /// <param name="entities">Отрезки</param>
    /// <param name="offset">Индекс начала разреза</param>
    /// <param name="length">Длина выделения</param>
    /// <returns>Разрешённые отрезки текста</returns>
    public static IEnumerable<MessageEntity> ResolveEntitiesForSlice(IEnumerable<MessageEntity> entities, int offset, int length) {
        var result = new List<MessageEntity>();
        foreach (var entity in entities) {
            var sectionEnd = entity.Offset + entity.Length;
            var sliceEnd = offset + length;
            if (entity.Offset < sliceEnd && sectionEnd > offset) {
                var newSection = new MessageEntity {
                    Offset = Math.Max(entity.Offset, offset) - offset,
                    Length = Math.Min(sectionEnd, sliceEnd) - Math.Max(entity.Offset, offset),
                    Type = entity.Type
                };
                result.Add(newSection);
            }
        }
        return result;
    }
}
