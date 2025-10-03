namespace Telegram.Bot.Messages.Test;
/// <summary>
///     Unit tests to <see cref="MessageSlice"/>
/// </summary>
public class MessageSliceHelperTest : AbstractTest {
    [TestCase(5, 10, 1)]
    [TestCase(10, 10, 1)]
    [TestCase(10 * 3, 10, 3)]
    [TestCase(10 + 1, 10, 2)]
    public async Task SplitMessageIntoSlices(int strLength, int maxSliceSize, int expectedSlicesNum) {
        await Console.Out.WriteLineAsync(strLength.ToString());
        await Console.Out.WriteLineAsync(maxSliceSize.ToString());
        await Console.Out.WriteLineAsync(expectedSlicesNum.ToString());
        var slice = GenerateMessageSlice(strLength);
        await Console.Out.WriteLineAsync(slice.Content);
        var slices = MessageSliceHelper.Split(slice, MessageSplitStrategy.Word, maxSliceSize);
        await Console.Out.WriteLineAsync(slices.Length.ToString());
        for (var i = 0; i < slices.Length; ++i) {
            await Console.Out.WriteLineAsync($"{i+1}: {slices[i]}");
        }
        Assert.That(slices, Has.Length.EqualTo(expectedSlicesNum));
    }
    [TestCase(10, 10, 20, 1)]
    [TestCase(11, 10, 20, 2)]
    [TestCase(20, 10, 20, 2)]
    [TestCase(30, 10, 20, 2)]
    [TestCase(31, 10, 20, 3)]
    public async Task SplitMessageWithFirstSliceMaxSize(int strLength, int firstMaxSliceSize, int maxSliceSize, int expectedSlicesNum) {
        await Console.Out.WriteLineAsync(strLength.ToString());
        await Console.Out.WriteLineAsync(firstMaxSliceSize.ToString());
        await Console.Out.WriteLineAsync(maxSliceSize.ToString());
        await Console.Out.WriteLineAsync(expectedSlicesNum.ToString());
        var slice = GenerateMessageSlice(strLength);
        await Console.Out.WriteLineAsync(slice.Content);
        var slices = MessageSliceHelper.Split(slice, MessageSplitStrategy.Word, firstMaxSliceSize, maxSliceSize);
        await Console.Out.WriteLineAsync(slices.Length.ToString());
        for (var i = 0; i < slices.Length; ++i) {
            await Console.Out.WriteLineAsync($"{i+1}: {slices[i]}");
        }
        Assert.That(slices, Has.Length.EqualTo(expectedSlicesNum));
    }
    [TestCase("hello", 0)]
    [TestCase("hello", 1, "h", "e", "l", "l", "o")]
    [TestCase("hello world", 8, "hello ", "world")]
    [TestCase("hello my world", 7, "hello ", "my ", "world")]
    [TestCase("hello my world", 6, "hello ", "my ", "world")]
    [TestCase("hello my world", 8, "hello my", " world")]
    public async Task SplitMessageIntoSlicesByWordStrategy(string str, int sliceSize, params string[] expectedSlices) {
        var slice = GenerateMessageSlice(str);
        await Console.Out.WriteLineAsync(str);
        foreach (var expectedSlice in expectedSlices) {
            await Console.Out.WriteLineAsync(expectedSlice);
        }
        var slices = MessageSliceHelper.Split(slice, MessageSplitStrategy.Word, sliceSize);
        foreach (var item in slices) {
            await Console.Out.WriteLineAsync(item.ToString());
        }
        var contentSlices = slices.Select(item => item.Content).ToArray();
        Assert.That(expectedSlices, Is.EqualTo(contentSlices).AsCollection);
    }
    [TestCase("hello", 0)]
    [TestCase("hello", 1, "h", "e", "l", "l", "o")]
    [TestCase("hello\nworld", 8, "hello", "\nworld")]
    [TestCase("hello\nmy\nworld", 7, "hello", "\nmy", "\nworld")]
    [TestCase("hello\nmy\nworld", 6, "hello", "\nmy", "\nworld")]
    [TestCase("hello my world", 8, "hello my", " world")]
    public async Task SplitMessageIntoSlicesByLineStrategy(string str, int sliceSize, params string[] expectedSlices) {
        var slice = GenerateMessageSlice(str);
        await Console.Out.WriteLineAsync(str);
        foreach (var expectedSlice in expectedSlices) {
            await Console.Out.WriteLineAsync(expectedSlice);
        }
        var slices = MessageSliceHelper.Split(slice, MessageSplitStrategy.Line, sliceSize);
        foreach (var item in slices) {
            await Console.Out.WriteLineAsync(item.ToString());
        }
        var contentSlices = slices.Select(item => item.Content).ToArray();
        Assert.That(expectedSlices, Is.EqualTo(contentSlices).AsCollection);
    }
    [TestCase("hello", 0)]
    [TestCase("hello", 1, "h", "e", "l", "l", "o")]
    [TestCase("hello\n\nworld", 8, "hello", "\n\nworld")]
    [TestCase("hello my\nworld", 7, "hello m", "y\nworld")]
    [TestCase("hello\n\nmy\n\nworld", 6, "hello\n", "\nmy", "\n\nworl", "d")]
    [TestCase("hello\n\nmy world", 10, "hello", "\n\nmy world")]
    public async Task SplitMessageIntoSlicesByParagraphStrategy(string str, int sliceSize, params string[] expectedSlices) {
        var slice = GenerateMessageSlice(str);
        await Console.Out.WriteLineAsync(str);
        foreach (var expectedSlice in expectedSlices) {
            await Console.Out.WriteLineAsync(expectedSlice);
        }
        var slices = MessageSliceHelper.Split(slice, MessageSplitStrategy.Paragraph, sliceSize);
        foreach (var item in slices) {
            await Console.Out.WriteLineAsync(item.ToString());
        }
        var contentSlices = slices.Select(item => item.Content).ToArray();
        Assert.That(expectedSlices, Is.EqualTo(contentSlices).AsCollection);
    }
}