namespace Telegram.Bot.Messages.Test;
/// <summary>
///     Unit tests to <see cref="MessageBuilder"/>
/// </summary>
public class MessageBuilderTest : AbstractTest {
    /// <summary>
    ///     Test for message builder initialization with characters provided in constructor
    /// </summary>
    [TestCase("")]
    [TestCase("hello")]
    [TestCase("hello   ")]
    [TestCase("   hello   ")]
    [TestCase("hello\nworld")]
    [TestCase("hello\nmy\nworld")]
    [TestCase("hello\nmy\nworld\n")]
    [TestCase("hello\nmy\nworld\n\n")]
    [TestCase("hello\nmy\nworld\n\n\n")]
    public async Task InitialContentViaConstructorTest(string inputAndExpectedResult) {
        await Console.Out.WriteLineAsync(inputAndExpectedResult);
        var message = new MessageBuilder(inputAndExpectedResult);
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(inputAndExpectedResult));
    }
    /// <summary>
    ///     Test for sequential character appending to message builder
    /// </summary>
    [TestCase("")]
    [TestCase("hello world", "h", "e", "l", "l", "o", " ", "w", "o", "r", "l", "d")]
    [TestCase("hello\nworld\n", "h", "e", "l", "l", "o", "\n", "w", "o", "r", "l", "d", "\n")]
    [TestCase("hello\nworld\n  ", "h", "e", "l", "l", "o", "\n", "w", "o", "r", "l", "d", "\n", " ", " ")]
    public async Task AppendSymbolsTest(string expectedResult, params string[] symbols) {
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder();
        foreach (var symbol in symbols) {
            await Console.Out.WriteAsync($"'{symbol}' ");
            message.Append(symbol);
        }
        await Console.Out.WriteLineAsync();
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(expectedResult));
    }
    /// <summary>
    ///     Test for appending characters with NL symbol at the end
    /// </summary>
    [TestCase("\n")]
    [TestCase("hello\n", "hello")]
    [TestCase("hello world\n\n", "hello world\n")]
    public async Task AppendSymbolsWithIndentTest(string expectedResult, params string[] symbols) {
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder();
        foreach (var symbol in symbols) {
            await Console.Out.WriteAsync($"'{symbol}' ");
            message.Append(symbol);
        }
        message.AppendLine();
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(expectedResult));
    }
    /// <summary>
    ///     Test for adding fields to message
    /// </summary>
    [TestCase("field1: value1\n", "field1", "value1")]
    [TestCase("field1: value1\nfield2: value2\n", "field1", "value1", "field2", "value2")]
    public async Task AppendFieldsTest(string expectedResult, params string[] fieldsAndValues) {
        if (fieldsAndValues.Length % 2 != 0) {
            Assert.Fail("Invalid input arguments for test");
        }
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder();
        for (var i = 0; i < fieldsAndValues.Length; i += 2) {
            var field = fieldsAndValues[i];
            var value = fieldsAndValues[i+1];
            await Console.Out.WriteLineAsync($"{field}' '{value}'");
            message.AppendField(field, value);
        }
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(expectedResult));
    }
    /// <summary>
    ///     Test for adding styled text segments
    /// </summary>
    [Test]
    [Repeat(20)]
    public async Task AppendEntitiesTest() {
        var message = new MessageBuilder();
        var lines = GenerateRandomInt(5, 10);
        var text = string.Empty;
        for (var i = 0; i < lines; ++i) {
            text += GenerateRandomString() + "\n";
        }
        await Console.Out.WriteLineAsync(text);
        var entitiesNumber = 0;
        var entitiesSlices = new List<(int, int)>();
        for (var i = 0; i < text.Length;) {
            var randomEntityEnd = GenerateRandomInt(i, text.Length - 1);
            randomEntityEnd += 1;
            var entitySlice = text[i..randomEntityEnd];
            var entitySlicePos = (i, randomEntityEnd);
            entitiesSlices.Add(entitySlicePos);
            var entity = GenerateMessageEntityType();
            message.AppendEntity(entitySlice, entity, false);
            await Console.Out.WriteLineAsync($"{entitySlicePos.i}, {entitySlicePos.randomEntityEnd} -> '{entitySlice}'");
            i = randomEntityEnd;
            entitiesNumber++;
        }
        var slice = message.Build();
        await Console.Out.WriteLineAsync(slice.Content);
        Assert.That(slice.Content, Is.EquivalentTo(text));
        await Console.Out.WriteLineAsync(entitiesNumber.ToString());
        await Console.Out.WriteLineAsync(slice.Entities.Length.ToString());
        Assert.That(slice.Entities, Has.Length.EqualTo(entitiesNumber));
        for (var i = 0; i < slice.Entities.Length; ++i) {
            var entity = slice.Entities[i];
            var expectedEntity = entitiesSlices[i];
            var expectedEntityLength = expectedEntity.Item2 - expectedEntity.Item1;
            await Console.Out.WriteLineAsync($"({entity.Offset}, {entity.Length}) == ({expectedEntity.Item1}, {expectedEntityLength})");
            using (Assert.EnterMultipleScope()) {
                Assert.That(entity.Offset, Is.EqualTo(expectedEntity.Item1));
                Assert.That(entity.Length, Is.EqualTo(expectedEntityLength));
            };
        }
    }
    /// <summary>
    ///     Test for adding fields with bold headers
    /// </summary>
    [TestCase("field1: value1\n", "field1", "value1")]
    [TestCase("field1: value1\nfield2: value2\n", "field1", "value1", "field2", "value2")]
    public async Task AppendFieldsWithBoldTitlesTest(string expectedResult, params string[] fieldsAndValues) {
        if (fieldsAndValues.Length % 2 != 0) {
            Assert.Fail("Invalid input arguments for test");
        }
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder();
        var entities = new List<(int, int)>();
        for (var i = 0; i < fieldsAndValues.Length; i += 2) {
            var field = fieldsAndValues[i];
            var value = fieldsAndValues[i+1];
            var entity = (message.Length, message.Length + field.Length);
            entities.Add(entity);
            await Console.Out.WriteLineAsync($"{field}' '{value}' (bold title: [{entity.Length}, {entity.Item2}])");
            message.AppendField(field, value, boldTitle: true);
        }
        var slice = message.Build();
        Assert.That(slice.Entities, Has.Length.EqualTo(entities.Count));
        for (var i = 0; i < slice.Entities.Length; ++i) {
            var entity = slice.Entities[i];
            var expectedEntity = entities[i];
            var expectedEntityLength = expectedEntity.Item2 - expectedEntity.Item1;
            await Console.Out.WriteLineAsync($"({entity.Offset}, {entity.Length}) == ({expectedEntity.Item1}, {expectedEntityLength})");
            using (Assert.EnterMultipleScope()) {
                Assert.That(entity.Offset, Is.EqualTo(expectedEntity.Item1));
                Assert.That(entity.Length, Is.EqualTo(expectedEntityLength));
            };
        }
    }
    /// <summary>
    ///     Test for adding paragraphs
    /// </summary>
    [TestCase("", false, "")]
    [TestCase("", true, "")]
    [TestCase("p1", false, "p1")]
    [TestCase("p1\n", true, "p1")]
    [TestCase("p1p2", false, "p1", "p2")]
    [TestCase("p1\np2\n", true, "p1", "p2")]
    [TestCase("p1p2p3", false, "p1", "p2", "p3")]
    [TestCase("p1\np2\np3\n", true, "p1", "p2", "p3")]
    public async Task AppendParagraphsTest(string expectedResult, bool nl, params string[] paragraphs) {
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder();
        foreach (var paragraph in paragraphs) {
            await Console.Out.WriteLineAsync($"ADD PARAGRAPH: '{paragraph}'");
            message.AppendParagraph(printer => printer.Append(paragraph), nl);
        }
        var slice = message.Build();
        await Console.Out.WriteLineAsync();
        await Console.Out.WriteLineAsync(slice.Content);
        Assert.That(slice.Content, Is.EquivalentTo(expectedResult));
    }
    /// <summary>
    ///     Test for adding paragraphs with styled segments
    /// </summary>
    [Test]
    [Repeat(10)]
    public async Task AppendParagraphsWithEntitiesTest() {
        var message = new MessageBuilder();
        var entities = new List<(int, int)>();
        var paragraphsNumber = GenerateRandomInt(5, 20);
        await Console.Out.WriteLineAsync(paragraphsNumber.ToString());
        for (var i = 0; i < paragraphsNumber; ++i) {
            var paragraphContent = GenerateRandomString();
            var paragraphEndIndex = GenerateRandomInt(i, paragraphContent.Length - 1);
            var paragraphSlice = (message.Length, paragraphEndIndex);
            entities.Add(paragraphSlice);
            await Console.Out.WriteLineAsync($"{paragraphContent} <- ({paragraphSlice.Item1}, {paragraphSlice.Item2})");
            var entityType = GenerateMessageEntityType();
            message.AppendParagraph(paragraph => {
                var a = paragraphSlice;
                var styledTextSlice = paragraphContent[..paragraphEndIndex];
                paragraph.AppendEntity(styledTextSlice, entityType, false);
                var otherTextSlice = paragraphContent[paragraphEndIndex..];
                paragraph.Append(otherTextSlice);
            });
        }
        var slice = message.Build();
        await Console.Out.WriteLineAsync(slice.Entities.Length.ToString());
        Assert.That(slice.Entities, Has.Length.EqualTo(entities.Count));
        for (var i = 0; i < paragraphsNumber; ++i) {
            var actualEntity = slice.Entities[i];
            var expectedEntity = entities[i];
            await Console.Out.WriteLineAsync($"({actualEntity.Offset}, {actualEntity.Length}) == ({expectedEntity.Item1}, {expectedEntity.Item2})");
            using (Assert.EnterMultipleScope()) {
                Assert.That(actualEntity.Offset, Is.EqualTo(expectedEntity.Item1));
                Assert.That(actualEntity.Length, Is.EqualTo(expectedEntity.Item2));
            }
        }
    }
    /// <summary>
    ///     Test for trimming excess NL characters at the end
    /// </summary>
    [TestCase("", "")]
    [TestCase(" \n\n", " \n")]
    [TestCase("hello world", "hello world")]
    [TestCase("hello world\n", "hello world\n")]
    [TestCase("hello world\n\n", "hello world\n")]
    [TestCase("hello world\n\n\n", "hello world\n")]
    public async Task TrimMessageTest(string input, string expectedResult) {
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder(input);
        message.TrimEnd();
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(expectedResult));
    }
    /// <summary>
    ///     Test for trimming excess NL characters in message with styled segments
    /// </summary>
    [TestCase("hello world", 0, 11, "hello world")]
    [TestCase("hello world\n", 0, 12, "hello world\n")]
    [TestCase("hello world\n\n", 0, 13, "hello world\n")]
    [TestCase("hello world\n\n\n", 0, 13, "hello world\n")]
    public async Task TrimMessageWithEntitiesTest(string input, int entityStartIndex, int entityEndIndex, string expectedResult) {
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder();
        var beforeEntity = input[..entityStartIndex];
        message.Append(beforeEntity);
        var entityContent = input[entityStartIndex..entityEndIndex];
        message.AppendCode(entityContent, false);
        var afterEntity = input[entityEndIndex..];
        message.Append(afterEntity);
        message.TrimEnd();
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(expectedResult));
    }
    /// <summary>
    ///     Test for trimming excess NL characters at the end
    /// </summary>
    [TestCase("hello world\n\n", "hello world\nmy", "my")]
    [TestCase("hello world\n\n", "hello world\nmyworld", "my", "world")]
    public async Task TrimMessageAndAddEntitiesTest(string input, string expectedResult, params string[] entities) {
        await Console.Out.WriteLineAsync(expectedResult);
        var message = new MessageBuilder(input);
        message.TrimEnd();
        var lastEntityIndex = input.Length - 1;
        var expectedEntities = new List<(int, int)>();
        foreach (var entity in entities) {
            var entityPos = (lastEntityIndex, entity.Length);
            expectedEntities.Add(entityPos);
            await Console.Out.WriteAsync($"'{entity}' <- ({entityPos.lastEntityIndex}, {entityPos.Item2})");
            message.AppendCode(entity, false);
            lastEntityIndex += entity.Length;
        }
        await Console.Out.WriteLineAsync();
        var result = message.Build();
        await Console.Out.WriteLineAsync(result.Content);
        Assert.That(result.Content, Is.EquivalentTo(expectedResult));
        await Console.Out.WriteLineAsync(expectedEntities.Count.ToString());
        await Console.Out.WriteLineAsync(result.Entities.Length.ToString());
        Assert.That(result.Entities, Has.Length.EqualTo(expectedEntities.Count));
        for (var i = 0; i < result.Entities.Length; ++i) {
            var actualEntity = result.Entities[i];
            var expectedEntity = expectedEntities[i];
            await Console.Out.WriteLineAsync($"({actualEntity.Offset}, {actualEntity.Length}) == ({expectedEntity.Item1}, {expectedEntity.Item2})");
            using (Assert.EnterMultipleScope()) {
                Assert.That(actualEntity.Offset, Is.EqualTo(expectedEntity.Item1));
                Assert.That(actualEntity.Length, Is.EqualTo(expectedEntity.Item2));
            }
        }
    }
}