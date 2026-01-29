using DS_OS.DataStructer;

namespace DS_OS_Test;

[TestFixture]
public class FileTreeTests
{
    private FileTree _fileTree;

    [SetUp]
    public void Setup()
    {
        _fileTree = new FileTree();
    }

    [Test]
    public void Root_IsInitialized()
    {
        // Assert
        Assert.That(_fileTree.Root, Is.Not.Null);
        Assert.That(_fileTree.Root.IsRoot, Is.True);
        Assert.That(_fileTree.Root.Name, Is.EqualTo("/"));
    }

    [Test]
    public void CreateFile_InRoot_CreatesFileSuccessfully()
    {
        // Act
        var result = _fileTree.CreateFile("/", "test.txt", 1024);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/test.txt"), Is.True);
    }

    [Test]
    public void CreateFile_InSubdirectory_CreatesFileSuccessfully()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "folder");

        // Act
        var result = _fileTree.CreateFile("/folder", "test.txt", 512);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/folder/test.txt"), Is.True);
    }

    [Test]
    public void CreateFile_DuplicateName_ReturnsFalse()
    {
        // Arrange
        _fileTree.CreateFile("/", "test.txt", 100);

        // Act
        var result = _fileTree.CreateFile("/", "test.txt", 200);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void CreateDirectory_InRoot_CreatesDirectorySuccessfully()
    {
        // Act
        var result = _fileTree.CreateDirectory("/", "mydir");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/mydir"), Is.True);
        var node = _fileTree.FindNode("/mydir");
        Assert.That(node?.IsDirectory, Is.True);
    }

    [Test]
    public void CreateDirectory_Nested_CreatesDirectorySuccessfully()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "parent");

        // Act
        var result = _fileTree.CreateDirectory("/parent", "child");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/parent/child"), Is.True);
    }

    [Test]
    public void Delete_ExistingFile_DeletesSuccessfully()
    {
        // Arrange
        _fileTree.CreateFile("/", "test.txt", 100);

        // Act
        var result = _fileTree.Delete("/test.txt");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/test.txt"), Is.False);
    }

    [Test]
    public void Delete_ExistingDirectory_DeletesSuccessfully()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "mydir");

        // Act
        var result = _fileTree.Delete("/mydir");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/mydir"), Is.False);
    }

    [Test]
    public void Delete_NonExistentPath_ReturnsFalse()
    {
        // Act
        var result = _fileTree.Delete("/nonexistent");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Delete_Root_ReturnsFalse()
    {
        // Act
        var result = _fileTree.Delete("/");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Exists_ExistingPath_ReturnsTrue()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "testdir");

        // Act
        var result = _fileTree.Exists("/testdir");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void Exists_NonExistentPath_ReturnsFalse()
    {
        // Act
        var result = _fileTree.Exists("/nonexistent");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void Exists_Root_ReturnsTrue()
    {
        // Act
        var result = _fileTree.Exists("/");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void FindNode_ExistingPath_ReturnsNode()
    {
        // Arrange
        _fileTree.CreateFile("/", "test.txt", 100);

        // Act
        var node = _fileTree.FindNode("/test.txt");

        // Assert
        Assert.That(node, Is.Not.Null);
        Assert.That(node!.Name, Is.EqualTo("test.txt"));
    }

    [Test]
    public void FindNode_NonExistentPath_ReturnsNull()
    {
        // Act
        var node = _fileTree.FindNode("/nonexistent");

        // Assert
        Assert.That(node, Is.Null);
    }

    [Test]
    public void ListDirectory_WithFiles_ReturnsAllChildren()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "dir");
        _fileTree.CreateFile("/dir", "file1.txt", 100);
        _fileTree.CreateFile("/dir", "file2.txt", 200);
        _fileTree.CreateDirectory("/dir", "subdir");

        // Act
        var contents = _fileTree.ListDirectory("/dir").ToList();

        // Assert
        Assert.That(contents.Count, Is.EqualTo(3));
    }

    [Test]
    public void GetFiles_ReturnsOnlyFiles()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "dir");
        _fileTree.CreateFile("/dir", "file1.txt", 100);
        _fileTree.CreateFile("/dir", "file2.txt", 200);
        _fileTree.CreateDirectory("/dir", "subdir");

        // Act
        var files = _fileTree.GetFiles("/dir").ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(2));
        Assert.That(files.All(f => f.IsFile), Is.True);
    }

    [Test]
    public void GetDirectories_ReturnsOnlyDirectories()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "dir");
        _fileTree.CreateFile("/dir", "file.txt", 100);
        _fileTree.CreateDirectory("/dir", "subdir1");
        _fileTree.CreateDirectory("/dir", "subdir2");

        // Act
        var dirs = _fileTree.GetDirectories("/dir").ToList();

        // Assert
        Assert.That(dirs.Count, Is.EqualTo(2));
        Assert.That(dirs.All(d => d.IsDirectory), Is.True);
    }

    [Test]
    public void Move_File_MovesSuccessfully()
    {
        // Arrange
        _fileTree.CreateFile("/", "test.txt", 100);
        _fileTree.CreateDirectory("/", "newdir");

        // Act
        var result = _fileTree.Move("/test.txt", "/newdir/test.txt");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/test.txt"), Is.False);
        Assert.That(_fileTree.Exists("/newdir/test.txt"), Is.True);
    }

    [Test]
    public void Move_Directory_MovesSuccessfully()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "olddir");
        _fileTree.CreateFile("/olddir", "file.txt", 100);
        _fileTree.CreateDirectory("/", "newparent");

        // Act
        var result = _fileTree.Move("/olddir", "/newparent/olddir");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/olddir"), Is.False);
        Assert.That(_fileTree.Exists("/newparent/olddir"), Is.True);
        Assert.That(_fileTree.Exists("/newparent/olddir/file.txt"), Is.True);
    }

    [Test]
    public void Copy_File_CopiesSuccessfully()
    {
        // Arrange
        _fileTree.CreateFile("/", "original.txt", 1024);
        _fileTree.CreateDirectory("/", "copydir");

        // Act
        var result = _fileTree.Copy("/original.txt", "/copydir/copy.txt");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/original.txt"), Is.True);
        Assert.That(_fileTree.Exists("/copydir/copy.txt"), Is.True);
    }

    [Test]
    public void Copy_Directory_CopiesRecursively()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "source");
        _fileTree.CreateFile("/source", "file1.txt", 100);
        _fileTree.CreateDirectory("/source", "subdir");
        _fileTree.CreateFile("/source/subdir", "file2.txt", 200);
        _fileTree.CreateDirectory("/", "dest");

        // Act
        var result = _fileTree.Copy("/source", "/dest/source");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_fileTree.Exists("/source"), Is.True);
        Assert.That(_fileTree.Exists("/dest/source"), Is.True);
        Assert.That(_fileTree.Exists("/dest/source/file1.txt"), Is.True);
        Assert.That(_fileTree.Exists("/dest/source/subdir"), Is.True);
        Assert.That(_fileTree.Exists("/dest/source/subdir/file2.txt"), Is.True);
    }

    [Test]
    public void GetDirectorySize_EmptyDirectory_ReturnsZero()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "empty");

        // Act
        var size = _fileTree.GetDirectorySize("/empty");

        // Assert
        Assert.That(size, Is.EqualTo(0));
    }

    [Test]
    public void GetDirectorySize_WithFiles_ReturnsCorrectSize()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "dir");
        _fileTree.CreateFile("/dir", "file1.txt", 100);
        _fileTree.CreateFile("/dir", "file2.txt", 200);
        _fileTree.CreateDirectory("/dir", "subdir");
        _fileTree.CreateFile("/dir/subdir", "file3.txt", 300);

        // Act
        var size = _fileTree.GetDirectorySize("/dir");

        // Assert
        Assert.That(size, Is.EqualTo(600));
    }

    [Test]
    public void GetParent_ReturnsCorrectParent()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "parent");
        _fileTree.CreateFile("/parent", "child.txt", 100);

        // Act
        var parent = _fileTree.GetParent("/parent/child.txt");

        // Assert
        Assert.That(parent, Is.Not.Null);
        Assert.That(parent!.Name, Is.EqualTo("parent"));
    }

    [Test]
    public void GetFullPath_ReturnsCorrectPath()
    {
        // Arrange
        _fileTree.CreateDirectory("/", "dir1");
        _fileTree.CreateDirectory("/dir1", "dir2");
        var node = _fileTree.FindNode("/dir1/dir2");

        // Act
        var path = _fileTree.GetFullPath(node!);

        // Assert
        Assert.That(path, Is.EqualTo("/dir1/dir2"));
    }

    [Test]
    public void ComplexScenario_MultipleOperations_WorksCorrectly()
    {
        // Create directory structure
        _fileTree.CreateDirectory("/", "docs");
        _fileTree.CreateDirectory("/docs", "work");
        _fileTree.CreateDirectory("/docs", "personal");
        
        // Create files
        _fileTree.CreateFile("/docs/work", "report.pdf", 2048);
        _fileTree.CreateFile("/docs/work", "presentation.ppt", 4096);
        _fileTree.CreateFile("/docs/personal", "photo.jpg", 1024);
        
        // Verify structure
        Assert.That(_fileTree.GetFiles("/docs/work").Count(), Is.EqualTo(2));
        Assert.That(_fileTree.GetDirectories("/docs").Count(), Is.EqualTo(2));
        
        // Move file
        _fileTree.Move("/docs/work/report.pdf", "/docs/personal/report.pdf");
        Assert.That(_fileTree.Exists("/docs/personal/report.pdf"), Is.True);
        Assert.That(_fileTree.Exists("/docs/work/report.pdf"), Is.False);
        
        // Calculate sizes
        var workSize = _fileTree.GetDirectorySize("/docs/work");
        var personalSize = _fileTree.GetDirectorySize("/docs/personal");
        var totalSize = _fileTree.GetDirectorySize("/docs");
        
        Assert.That(workSize, Is.EqualTo(4096));
        Assert.That(personalSize, Is.EqualTo(3072));
        Assert.That(totalSize, Is.EqualTo(7168));
        
        // Delete directory
        _fileTree.Delete("/docs/work");
        Assert.That(_fileTree.Exists("/docs/work"), Is.False);
        Assert.That(_fileTree.GetDirectories("/docs").Count(), Is.EqualTo(1));
    }
}
