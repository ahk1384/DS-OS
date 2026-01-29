using DS_OS.Engine.CommandHandler.BaseClass;

namespace DS_OS_Test;

[TestFixture]
public class CommandTests
{
    [Test]
    public void Constructor_CreatesCommand_WithTypeAndParameters()
    {
        // Arrange
        var parameters = new Dictionary<string, string>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        // Act
        var command = new Command(CommandType.PROCESSCREATE, parameters);

        // Assert
        Assert.That(command.Type, Is.EqualTo(CommandType.PROCESSCREATE));
        Assert.That(command.Parameters, Is.Not.Null);
        Assert.That(command.Parameters.Count, Is.EqualTo(2));
        Assert.That(command.Parameters["key1"], Is.EqualTo("value1"));
        Assert.That(command.Parameters["key2"], Is.EqualTo("value2"));
    }

    [Test]
    public void Constructor_WithEmptyParameters_CreatesCommand()
    {
        // Arrange
        var parameters = new Dictionary<string, string>();

        // Act
        var command = new Command(CommandType.SHUTDOWN, parameters);

        // Assert
        Assert.That(command.Type, Is.EqualTo(CommandType.SHUTDOWN));
        Assert.That(command.Parameters, Is.Not.Null);
        Assert.That(command.Parameters.Count, Is.EqualTo(0));
    }

    [Test]
    public void AddParams_AddsNewParameter()
    {
        // Arrange
        var parameters = new Dictionary<string, string>();
        var command = new Command(CommandType.FILECREATE, parameters);

        // Act
        command.AddParams("path", "/home/user");
        command.AddParams("name", "test.txt");

        // Assert
        Assert.That(command.Parameters.Count, Is.EqualTo(2));
        Assert.That(command.Parameters["path"], Is.EqualTo("/home/user"));
        Assert.That(command.Parameters["name"], Is.EqualTo("test.txt"));
    }

    [Test]
    public void AddParams_DuplicateKey_ThrowsException()
    {
        // Arrange
        var parameters = new Dictionary<string, string> { { "key", "value1" } };
        var command = new Command(CommandType.FILECREATE, parameters);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => command.AddParams("key", "value2"));
    }

    [Test]
    public void Parameters_PreserveOriginalDictionary()
    {
        // Arrange
        var parameters = new Dictionary<string, string>
        {
            { "pid", "123" },
            { "priority", "5" }
        };

        // Act
        var command = new Command(CommandType.PROCESSCREATE, parameters);

        // Assert - Verify parameters are accessible
        Assert.That(command.Parameters["pid"], Is.EqualTo("123"));
        Assert.That(command.Parameters["priority"], Is.EqualTo("5"));
    }

    [Test]
    public void Command_AllCommandTypes_CanBeCreated()
    {
        // Test all command types can be instantiated
        var types = new[]
        {
            CommandType.None,
            CommandType.PROCESSCREATE,
            CommandType.PROCESSDELETE,
            CommandType.FILECREATE,
            CommandType.FILEDELETE,
            CommandType.SHUTDOWN
        };

        foreach (var type in types)
        {
            var command = new Command(type, new Dictionary<string, string>());
            Assert.That(command.Type, Is.EqualTo(type));
        }
    }
}

[TestFixture]
public class CommandTypeTests
{
    [Test]
    public void CommandType_HasExpectedValues()
    {
        // Assert - Verify all expected enum values exist
        Assert.That(Enum.IsDefined(typeof(CommandType), CommandType.None));
        Assert.That(Enum.IsDefined(typeof(CommandType), CommandType.PROCESSCREATE));
        Assert.That(Enum.IsDefined(typeof(CommandType), CommandType.PROCESSDELETE));
        Assert.That(Enum.IsDefined(typeof(CommandType), CommandType.FILECREATE));
        Assert.That(Enum.IsDefined(typeof(CommandType), CommandType.FILEDELETE));
        Assert.That(Enum.IsDefined(typeof(CommandType), CommandType.SHUTDOWN));
    }

    [Test]
    public void CommandType_None_IsZero()
    {
        // Assert
        Assert.That((int)CommandType.None, Is.EqualTo(0));
    }

    [Test]
    public void CommandType_AllValues_AreUnique()
    {
        // Arrange
        var values = Enum.GetValues(typeof(CommandType)).Cast<CommandType>().ToList();

        // Act
        var distinctValues = values.Distinct().ToList();

        // Assert
        Assert.That(distinctValues.Count, Is.EqualTo(values.Count));
    }
}
