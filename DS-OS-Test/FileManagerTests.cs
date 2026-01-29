using DS_OS.DataStructer;
using DS_OS.FileManager;

namespace DS_OS_Test;

[TestFixture]
public class FileManagerTests
{
    private IFileManager _fileManager = null!;

    [SetUp]
    public void Setup()
    {
        _fileManager = new DS_OS.FileManager.FileManager();
    }

    #region GetAllFilesRecursive Tests

    [Test]
    public void GetAllFilesRecursive_EmptyFileSystem_ReturnsEmpty()
    {
        // Act
        var files = _fileManager.GetAllFilesRecursive("/");

        // Assert
        Assert.That(files, Is.Empty);
    }

    [Test]
    public void GetAllFilesRecursive_SingleFile_ReturnsSingleFile()
    {
        // Arrange
        _fileManager.CreateFile("/", "test.txt", 100);

        // Act
        var files = _fileManager.GetAllFilesRecursive("/").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(1));
        Assert.That(files[0].Name, Is.EqualTo("test.txt"));
        Assert.That(files[0].FullPath, Is.EqualTo("/test.txt"));
    }

    [Test]
    public void GetAllFilesRecursive_MultipleFilesInRoot_ReturnsAllFiles()
    {
        // Arrange
        _fileManager.CreateFile("/", "file1.txt", 100);
        _fileManager.CreateFile("/", "file2.txt", 200);
        _fileManager.CreateFile("/", "file3.txt", 300);

        // Act
        var files = _fileManager.GetAllFilesRecursive("/").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(3));
        Assert.That(files.Select(f => f.Name), Contains.Item("file1.txt"));
        Assert.That(files.Select(f => f.Name), Contains.Item("file2.txt"));
        Assert.That(files.Select(f => f.Name), Contains.Item("file3.txt"));
    }

    [Test]
    public void GetAllFilesRecursive_NestedDirectories_ReturnsAllFiles()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "dir1");
        _fileManager.CreateDirectory("/dir1", "dir2");
        _fileManager.CreateDirectory("/dir1", "dir3");
        
        _fileManager.CreateFile("/", "root.txt", 100);
        _fileManager.CreateFile("/dir1", "file1.txt", 200);
        _fileManager.CreateFile("/dir1/dir2", "file2.txt", 300);
        _fileManager.CreateFile("/dir1/dir3", "file3.txt", 400);

        // Act
        var files = _fileManager.GetAllFilesRecursive("/").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(4));
        Assert.That(files.Select(f => f.Name), Contains.Item("root.txt"));
        Assert.That(files.Select(f => f.Name), Contains.Item("file1.txt"));
        Assert.That(files.Select(f => f.Name), Contains.Item("file2.txt"));
        Assert.That(files.Select(f => f.Name), Contains.Item("file3.txt"));
    }

    [Test]
    public void GetAllFilesRecursive_FromSubdirectory_ReturnsOnlySubdirectoryFiles()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "dir1");
        _fileManager.CreateDirectory("/", "dir2");
        
        _fileManager.CreateFile("/", "root.txt", 100);
        _fileManager.CreateFile("/dir1", "file1.txt", 200);
        _fileManager.CreateFile("/dir2", "file2.txt", 300);

        // Act
        var files = _fileManager.GetAllFilesRecursive("/dir1").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(1));
        Assert.That(files[0].Name, Is.EqualTo("file1.txt"));
    }

    [Test]
    public void GetAllFilesRecursive_DeepNesting_ReturnsAllFiles()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "level1");
        _fileManager.CreateDirectory("/level1", "level2");
        _fileManager.CreateDirectory("/level1/level2", "level3");
        _fileManager.CreateDirectory("/level1/level2/level3", "level4");
        
        _fileManager.CreateFile("/level1", "file1.txt", 100);
        _fileManager.CreateFile("/level1/level2", "file2.txt", 200);
        _fileManager.CreateFile("/level1/level2/level3", "file3.txt", 300);
        _fileManager.CreateFile("/level1/level2/level3/level4", "file4.txt", 400);

        // Act
        var files = _fileManager.GetAllFilesRecursive("/level1").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(4));
    }

    [Test]
    public void GetAllFilesRecursive_InvalidPath_ReturnsEmpty()
    {
        // Act
        var files = _fileManager.GetAllFilesRecursive("/nonexistent");

        // Assert
        Assert.That(files, Is.Empty);
    }

    [Test]
    public void GetAllFilesRecursive_OnlyDirectories_ReturnsEmpty()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "dir1");
        _fileManager.CreateDirectory("/", "dir2");
        _fileManager.CreateDirectory("/dir1", "dir3");

        // Act
        var files = _fileManager.GetAllFilesRecursive("/");

        // Assert
        Assert.That(files, Is.Empty);
    }

    [Test]
    public void GetAllFilesRecursive_VerifyFullPaths()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "projects");
        _fileManager.CreateDirectory("/projects", "app1");
        _fileManager.CreateFile("/projects/app1", "main.cs", 1024);
        _fileManager.CreateFile("/projects/app1", "config.json", 512);

        // Act
        var files = _fileManager.GetAllFilesRecursive("/").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(2));
        Assert.That(files.Select(f => f.FullPath), Contains.Item("/projects/app1/main.cs"));
        Assert.That(files.Select(f => f.FullPath), Contains.Item("/projects/app1/config.json"));
    }

    #endregion

    #region Navigation and Current Directory Tests

    [Test]
    public void GetCurrentDirectory_InitialState_ReturnsRoot()
    {
        // Act
        var currentDir = _fileManager.GetCurrentDirectory();

        // Assert
        Assert.That(currentDir, Is.EqualTo("/"));
    }

    [Test]
    public void ChangeDirectory_ValidPath_ChangesDirectory()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "home");

        // Act
        var result = _fileManager.ChangeDirectory("/home");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileManager.GetCurrentDirectory(), Is.EqualTo("/home"));
    }

    [Test]
    public void ChangeDirectory_InvalidPath_ReturnsFalse()
    {
        // Act
        var result = _fileManager.ChangeDirectory("/nonexistent");

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_fileManager.GetCurrentDirectory(), Is.EqualTo("/"));
    }

    [Test]
    public void ChangeDirectory_ToFile_ReturnsFalse()
    {
        // Arrange
        _fileManager.CreateFile("/", "file.txt", 100);

        // Act
        var result = _fileManager.ChangeDirectory("/file.txt");

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region Validation Tests

    [Test]
    public void IsValidPath_ValidPaths_ReturnsTrue()
    {
        // Act & Assert
        Assert.That(_fileManager.IsValidPath("/"), Is.True);
        Assert.That(_fileManager.IsValidPath("/home"), Is.True);
        Assert.That(_fileManager.IsValidPath("/home/user/documents"), Is.True);
    }

    [Test]
    public void IsValidPath_InvalidPaths_ReturnsFalse()
    {
        // Act & Assert
        Assert.That(_fileManager.IsValidPath(""), Is.False);
        Assert.That(_fileManager.IsValidPath("   "), Is.False);
        Assert.That(_fileManager.IsValidPath("home"), Is.False); // No leading slash
        Assert.That(_fileManager.IsValidPath(null!), Is.False);
    }

    [Test]
    public void IsValidFileName_ValidNames_ReturnsTrue()
    {
        // Act & Assert
        Assert.That(_fileManager.IsValidFileName("file.txt"), Is.True);
        Assert.That(_fileManager.IsValidFileName("document-2024.pdf"), Is.True);
        Assert.That(_fileManager.IsValidFileName("my_file.txt"), Is.True);
    }

    [Test]
    public void IsValidFileName_InvalidNames_ReturnsFalse()
    {
        // Act & Assert
        Assert.That(_fileManager.IsValidFileName(""), Is.False);
        Assert.That(_fileManager.IsValidFileName("   "), Is.False);
        Assert.That(_fileManager.IsValidFileName("file/name.txt"), Is.False); // Contains /
        Assert.That(_fileManager.IsValidFileName("CON"), Is.False); // Reserved name
        Assert.That(_fileManager.IsValidFileName("PRN"), Is.False); // Reserved name
    }

    #endregion

    #region Search Operations Tests

    [Test]
    public void Search_MatchingPattern_ReturnsMatchingNodes()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "documents");
        _fileManager.CreateFile("/documents", "report.txt", 100);
        _fileManager.CreateFile("/documents", "report.pdf", 200);
        _fileManager.CreateFile("/documents", "invoice.txt", 150);

        // Act
        var results = _fileManager.Search("report").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results.Select(r => r.Name), Contains.Item("report.txt"));
        Assert.That(results.Select(r => r.Name), Contains.Item("report.pdf"));
    }

    [Test]
    public void FindByName_ExactMatch_ReturnsMatchingFiles()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "dir1");
        _fileManager.CreateDirectory("/", "dir2");
        _fileManager.CreateFile("/dir1", "test.txt", 100);
        _fileManager.CreateFile("/dir2", "test.txt", 200);

        // Act
        var results = _fileManager.FindByName("test.txt").ToList();

        // Assert
        Assert.That(results.Count, Is.EqualTo(2));
        Assert.That(results.All(r => r.Name.Equals("test.txt", StringComparison.OrdinalIgnoreCase)), Is.True);
    }

    #endregion

    #region File Properties Tests

    [Test]
    public void GetCreatedTime_ExistingFile_ReturnsValidTime()
    {
        // Arrange
        var before = DateTime.Now;
        _fileManager.CreateFile("/", "test.txt", 100);
        var after = DateTime.Now;

        // Act
        var createdTime = _fileManager.GetCreatedTime("/test.txt");

        // Assert
        Assert.That(createdTime, Is.GreaterThanOrEqualTo(before));
        Assert.That(createdTime, Is.LessThanOrEqualTo(after));
    }

    [Test]
    public void IsFile_File_ReturnsTrue()
    {
        // Arrange
        _fileManager.CreateFile("/", "test.txt", 100);

        // Act & Assert
        Assert.That(_fileManager.IsFile("/test.txt"), Is.True);
    }

    [Test]
    public void IsFile_Directory_ReturnsFalse()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "testdir");

        // Act & Assert
        Assert.That(_fileManager.IsFile("/testdir"), Is.False);
    }

    [Test]
    public void IsDirectory_Directory_ReturnsTrue()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "testdir");

        // Act & Assert
        Assert.That(_fileManager.IsDirectory("/testdir"), Is.True);
    }

    [Test]
    public void IsDirectory_File_ReturnsFalse()
    {
        // Arrange
        _fileManager.CreateFile("/", "test.txt", 100);

        // Act & Assert
        Assert.That(_fileManager.IsDirectory("/test.txt"), Is.False);
    }

    #endregion

    #region Rename Tests

    [Test]
    public void Rename_ValidName_RenamesSuccessfully()
    {
        // Arrange
        _fileManager.CreateFile("/", "oldname.txt", 100);

        // Act
        var result = _fileManager.Rename("/oldname.txt", "newname.txt");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileManager.Exists("/newname.txt"), Is.True);
        Assert.That(_fileManager.Exists("/oldname.txt"), Is.False);
    }

    [Test]
    public void Rename_DuplicateName_ReturnsFalse()
    {
        // Arrange
        _fileManager.CreateFile("/", "file1.txt", 100);
        _fileManager.CreateFile("/", "file2.txt", 200);

        // Act
        var result = _fileManager.Rename("/file1.txt", "file2.txt");

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_fileManager.Exists("/file1.txt"), Is.True);
    }

    [Test]
    public void Rename_InvalidName_ReturnsFalse()
    {
        // Arrange
        _fileManager.CreateFile("/", "test.txt", 100);

        // Act
        var result = _fileManager.Rename("/test.txt", "invalid/name.txt");

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region Clear and Helper Tests

    [Test]
    public void Clear_RemovesAllFilesAndDirectories()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "dir1");
        _fileManager.CreateFile("/", "file1.txt", 100);
        _fileManager.CreateFile("/dir1", "file2.txt", 200);

        // Act
        ((DS_OS.FileManager.FileManager)_fileManager).Clear();

        // Assert
        Assert.That(_fileManager.GetAllFilesRecursive("/"), Is.Empty);
        Assert.That(_fileManager.GetDirectories("/"), Is.Empty);
    }

    [Test]
    public void GetTotalNodeCount_EmptyFileSystem_ReturnsOne()
    {
        // Act
        var count = ((DS_OS.FileManager.FileManager)_fileManager).GetTotalNodeCount();

        // Assert - Only root exists
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public void GetTotalNodeCount_WithFilesAndDirectories_ReturnsCorrectCount()
    {
        // Arrange
        _fileManager.CreateDirectory("/", "dir1");
        _fileManager.CreateDirectory("/", "dir2");
        _fileManager.CreateFile("/dir1", "file1.txt", 100);
        _fileManager.CreateFile("/dir2", "file2.txt", 200);

        // Act
        var count = ((DS_OS.FileManager.FileManager)_fileManager).GetTotalNodeCount();

        // Assert - Root + 2 dirs + 2 files = 5
        Assert.That(count, Is.EqualTo(5));
    }

    #endregion

    #region Integration Tests

    [Test]
    public void FileManager_CompleteWorkflow_WorksCorrectly()
    {
        // Create directory structure
        Assert.That(_fileManager.CreateDirectory("/", "projects"), Is.True);
        Assert.That(_fileManager.CreateDirectory("/projects", "app1"), Is.True);

        // Create files
        Assert.That(_fileManager.CreateFile("/projects/app1", "main.cs", 2048), Is.True);
        Assert.That(_fileManager.CreateFile("/projects/app1", "config.json", 512), Is.True);

        // Verify recursive file retrieval
        var allFiles = _fileManager.GetAllFilesRecursive("/").ToList();
        Assert.That(allFiles.Count, Is.EqualTo(2));

        // Verify paths
        Assert.That(allFiles.Any(f => f.FullPath == "/projects/app1/main.cs"), Is.True);
        Assert.That(allFiles.Any(f => f.FullPath == "/projects/app1/config.json"), Is.True);

        // Test search
        var searchResults = _fileManager.Search("main").ToList();
        Assert.That(searchResults.Count, Is.EqualTo(1));
        Assert.That(searchResults[0].Name, Is.EqualTo("main.cs"));

        // Test navigation
        Assert.That(_fileManager.ChangeDirectory("/projects"), Is.True);
        Assert.That(_fileManager.GetCurrentDirectory(), Is.EqualTo("/projects"));

        // Verify directory size
        var size = _fileManager.GetDirectorySize("/projects/app1");
        Assert.That(size, Is.EqualTo(2560));
    }

    #endregion
}
