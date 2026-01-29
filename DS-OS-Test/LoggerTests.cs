using DS_OS.DataStructer;
using DS_OS.Logger;
using DS_OS.FileManager;

namespace DS_OS_Test;

[TestFixture]
public class LoggerTests
{
    private ProcessLogger _logger = null!;
    private string _testLogPath = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new ProcessLogger();
        // Create a unique test log file path
        _testLogPath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
        
        // Use reflection to set the path for testing
        var pathField = typeof(ProcessLogger).GetField("path", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pathField?.SetValue(_logger, _testLogPath);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test file
        if (File.Exists(_testLogPath))
        {
            try
            {
                File.Delete(_testLogPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    #region Basic Logging Tests

    [Test]
    public void Log_SingleMessage_ReturnsTrue()
    {
        // Act
        var result = _logger.Log("Test message");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    [Ignore("Requires named pipe server to be running")]
    public void Log_MultipleMessages_ReturnsTrue()
    {
        // Arrange
        string[] messages = { "Message 1", "Message 2", "Message 3" };

        // Act
        var result = _logger.Log(messages);

        // Assert
        Assert.That(result, Is.True);
        // Note: Pipe sending might fail in test environment, but the method still returns true
    }

    [Test]
    public void SaveLog_CreatesFile()
    {
        // Arrange
        _logger.Log("Test message");

        // Act
        _logger.SaveLog();

        // Assert
        Assert.That(File.Exists(_testLogPath), Is.True);
    }

    [Test]
    public void SaveLog_WritesMessagesToFile()
    {
        // Arrange
        _logger.Log("Message 1");
        _logger.Log("Message 2");

        // Act
        _logger.SaveLog();

        // Assert
        var fileContent = File.ReadAllText(_testLogPath);
        Assert.That(fileContent, Does.Contain("Message 1"));
        Assert.That(fileContent, Does.Contain("Message 2"));
    }

    [Test]
    public void ClearLog_EmptiesLogFile()
    {
        // Arrange
        _logger.Log("Test message");
        _logger.SaveLog();

        // Act
        _logger.ClearLog();

        // Assert
        var fileContent = File.ReadAllText(_testLogPath);
        Assert.That(fileContent, Is.Empty);
    }

    #endregion

    #region Logging Workflow Tests

    [Test]
    public void LogWorkflow_MultipleLogsAndSaves_WorksCorrectly()
    {
        // First batch
        _logger.Log("Batch 1 - Message 1");
        _logger.Log("Batch 1 - Message 2");
        _logger.SaveLog();

        // Second batch
        _logger.Log("Batch 2 - Message 1");
        _logger.SaveLog();

        // Assert
        var fileContent = File.ReadAllText(_testLogPath);
        Assert.That(fileContent, Does.Contain("Batch 1 - Message 1"));
        Assert.That(fileContent, Does.Contain("Batch 1 - Message 2"));
        Assert.That(fileContent, Does.Contain("Batch 2 - Message 1"));
    }

    [Test]
    public void SaveLog_ClearsInternalBuffer()
    {
        // Arrange
        _logger.Log("Test message");
        _logger.SaveLog();

        // Clear the file to test if buffer was cleared
        _logger.ClearLog();

        // Act - Save again without adding new messages
        _logger.SaveLog();

        // Assert - File should remain empty (no duplicate messages)
        var lines = File.ReadAllLines(_testLogPath);
        var messageLines = lines.Where(l => l.Contains("Test message")).ToList();
        Assert.That(messageLines.Count, Is.EqualTo(0));
    }

    #endregion

    #region Shutdown Logging Tests

    [Test]
    [Ignore("Requires named pipe server to be running")]
    public void Log_ShutdownMessage_HandlesSpecialCase()
    {
        // Act
        var result = _logger.Log("SHUTDOWN");

        // Assert
        Assert.That(result, Is.True);
        // Note: Named pipe sending will timeout in test environment without server
        // This is expected behavior - the method still returns true
    }

    #endregion
}

[TestFixture]
public class FileLoggerTests
{
    private FileLogger _fileLogger = null!;
    private string _testLogPath = null!;

    [SetUp]
    public void Setup()
    {
        _fileLogger = new FileLogger();
        // Create a unique test log file path
        _testLogPath = Path.Combine(Path.GetTempPath(), $"test_file_log_{Guid.NewGuid()}.txt");
        
        // Use reflection to set the path for testing
        var pathField = typeof(FileLogger).GetField("path", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        pathField?.SetValue(_fileLogger, _testLogPath);
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up test file
        if (File.Exists(_testLogPath))
        {
            try
            {
                File.Delete(_testLogPath);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    #region FileNode Logging Tests

    [Test]
    public void Log_FileNodes_WritesToFile()
    {
        // Arrange
        var file1 = FileNode.CreateFile("file1.txt", 100);
        var file2 = FileNode.CreateFile("file2.txt", 200);
        
        // Create a simple structure for testing
        var root = FileNode.CreateRoot("/");
        root.AddChild(file1);
        root.AddChild(file2);
        
        var files = new List<FileNode> { file1, file2 };

        // Act
        _fileLogger.Log(files);

        // Assert
        Assert.That(File.Exists(_testLogPath), Is.True);
        var fileContent = File.ReadAllText(_testLogPath);
        Assert.That(fileContent, Does.Contain("file1.txt"));
        Assert.That(fileContent, Does.Contain("file2.txt"));
    }

    [Test]
    public void Log_EmptyFileList_DoesNotCrash()
    {
        // Arrange
        var emptyList = Enumerable.Empty<FileNode>();

        // Act & Assert
        Assert.DoesNotThrow(() => _fileLogger.Log(emptyList));
    }

    [Test]
    public void Log_FileNodesWithPaths_WritesCorrectPaths()
    {
        // Arrange
        var root = FileNode.CreateRoot("/");
        var dir = FileNode.CreateDirectory("projects");
        var file = FileNode.CreateFile("test.txt", 100);
        
        root.AddChild(dir);
        dir.AddChild(file);
        
        var files = new List<FileNode> { file };

        // Act
        _fileLogger.Log(files);

        // Assert
        var fileContent = File.ReadAllText(_testLogPath);
        Assert.That(fileContent, Does.Contain("/projects/test.txt"));
    }

    [Test]
    public void ClearLog_RemovesAllContent()
    {
        // Arrange
        _fileLogger.Log("Test content");
        _fileLogger.SaveLog();

        // Act
        _fileLogger.ClearLog();

        // Assert
        var fileContent = File.ReadAllText(_testLogPath);
        Assert.That(fileContent, Is.Empty);
    }

    #endregion
}
