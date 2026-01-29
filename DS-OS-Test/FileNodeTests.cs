using DS_OS.DataStructer;

namespace DS_OS_Test;

[TestFixture]
public class FileNodeTests
{
    [Test]
    public void Constructor_CreatesFile_SetsPropertiesCorrectly()
    {
        // Act
        var fileNode = new FileNode("test.txt", true);

        // Assert
        Assert.That(fileNode.Name, Is.EqualTo("test.txt"));
        Assert.That(fileNode.IsFile, Is.True);
        Assert.That(fileNode.IsDirectory, Is.False);
        Assert.That(fileNode.IsRoot, Is.False);
        Assert.That(fileNode.Parent, Is.Null);
        Assert.That(fileNode.Children, Is.Not.Null);
        Assert.That(fileNode.Children.Count, Is.EqualTo(0));
    }

    [Test]
    public void Constructor_CreatesDirectory_SetsPropertiesCorrectly()
    {
        // Act
        var dirNode = new FileNode("mydir", false);

        // Assert
        Assert.That(dirNode.Name, Is.EqualTo("mydir"));
        Assert.That(dirNode.IsFile, Is.False);
        Assert.That(dirNode.IsDirectory, Is.True);
        Assert.That(dirNode.IsRoot, Is.False);
    }

    [Test]
    public void Constructor_EmptyName_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new FileNode("", true));
        Assert.Throws<ArgumentException>(() => new FileNode("   ", false));
    }

    [Test]
    public void CreateRoot_CreatesRootDirectory()
    {
        // Act
        var root = FileNode.CreateRoot("root");

        // Assert
        Assert.That(root.Name, Is.EqualTo("root"));
        Assert.That(root.IsDirectory, Is.True);
        Assert.That(root.IsRoot, Is.True);
        Assert.That(root.IsFile, Is.False);
    }

    [Test]
    public void CreateFile_CreatesFileWithSize()
    {
        // Act
        var file = FileNode.CreateFile("doc.pdf", 1024);

        // Assert
        Assert.That(file.IsFile, Is.True);
        Assert.That(file.Name, Is.EqualTo("doc.pdf"));
        Assert.That(file.Size, Is.EqualTo(1024));
    }

    [Test]
    public void CreateDirectory_CreatesDirectory()
    {
        // Act
        var dir = FileNode.CreateDirectory("folder");

        // Assert
        Assert.That(dir.IsDirectory, Is.True);
        Assert.That(dir.Name, Is.EqualTo("folder"));
        Assert.That(dir.Size, Is.EqualTo(0));
    }

    [Test]
    public void AddChild_ToDirectory_AddsChildSuccessfully()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var child = FileNode.CreateFile("child.txt", 100);

        // Act
        var result = parent.AddChild(child);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(parent.Children.Count, Is.EqualTo(1));
        Assert.That(child.Parent, Is.EqualTo(parent));
        Assert.That(parent.Children.ContainsKey("child.txt"), Is.True);
    }

    [Test]
    public void AddChild_ToFile_ThrowsInvalidOperationException()
    {
        // Arrange
        var file = FileNode.CreateFile("file.txt", 100);
        var child = FileNode.CreateFile("child.txt", 50);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => file.AddChild(child));
    }

    [Test]
    public void AddChild_DuplicateName_ReturnsFalse()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var child1 = FileNode.CreateFile("duplicate.txt", 100);
        var child2 = FileNode.CreateFile("duplicate.txt", 200);

        // Act
        parent.AddChild(child1);
        var result = parent.AddChild(child2);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(parent.Children.Count, Is.EqualTo(1));
    }

    [Test]
    public void AddChild_NullChild_ThrowsArgumentNullException()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => parent.AddChild(null!));
    }

    [Test]
    public void RemoveChild_ExistingChild_RemovesSuccessfully()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var child = FileNode.CreateFile("child.txt", 100);
        parent.AddChild(child);

        // Act
        var result = parent.RemoveChild("child.txt");

        // Assert
        Assert.That(result, Is.True);
        Assert.That(parent.Children.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveChild_NonExistingChild_ReturnsFalse()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");

        // Act
        var result = parent.RemoveChild("nonexistent.txt");

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void RemoveChild_FromFile_ThrowsInvalidOperationException()
    {
        // Arrange
        var file = FileNode.CreateFile("file.txt", 100);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => file.RemoveChild("anything"));
    }

    [Test]
    public void FindChild_ExistingChild_ReturnsChild()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var child = FileNode.CreateFile("child.txt", 100);
        parent.AddChild(child);

        // Act
        var found = parent.FindChild("child.txt");

        // Assert
        Assert.That(found, Is.Not.Null);
        Assert.That(found, Is.EqualTo(child));
    }

    [Test]
    public void FindChild_NonExistingChild_ReturnsNull()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");

        // Act
        var found = parent.FindChild("nonexistent.txt");

        // Assert
        Assert.That(found, Is.Null);
    }

    [Test]
    public void GetFiles_ReturnsOnlyFiles()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var file1 = FileNode.CreateFile("file1.txt", 100);
        var file2 = FileNode.CreateFile("file2.txt", 200);
        var dir = FileNode.CreateDirectory("subdir");
        
        parent.AddChild(file1);
        parent.AddChild(file2);
        parent.AddChild(dir);

        // Act
        var files = parent.GetFiles().ToList();

        // Assert
        Assert.That(files.Count, Is.EqualTo(2));
        Assert.That(files.All(f => f.IsFile), Is.True);
    }

    [Test]
    public void GetDirectories_ReturnsOnlyDirectories()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var file = FileNode.CreateFile("file.txt", 100);
        var dir1 = FileNode.CreateDirectory("dir1");
        var dir2 = FileNode.CreateDirectory("dir2");
        
        parent.AddChild(file);
        parent.AddChild(dir1);
        parent.AddChild(dir2);

        // Act
        var dirs = parent.GetDirectories().ToList();

        // Assert
        Assert.That(dirs.Count, Is.EqualTo(2));
        Assert.That(dirs.All(d => d.IsDirectory), Is.True);
    }

    [Test]
    public void GetFullPath_SingleLevel_ReturnsCorrectPath()
    {
        // Arrange
        var root = FileNode.CreateRoot("root");
        var child = FileNode.CreateFile("file.txt", 100);
        root.AddChild(child);

        // Act
        var path = child.GetFullPath();

        // Assert
        Assert.That(path, Is.EqualTo("root/file.txt"));
    }

    [Test]
    public void GetFullPath_MultipleLevel_ReturnsCorrectPath()
    {
        // Arrange
        var root = FileNode.CreateRoot("root");
        var dir1 = FileNode.CreateDirectory("dir1");
        var dir2 = FileNode.CreateDirectory("dir2");
        var file = FileNode.CreateFile("file.txt", 100);
        
        root.AddChild(dir1);
        dir1.AddChild(dir2);
        dir2.AddChild(file);

        // Act
        var path = file.GetFullPath();

        // Assert
        Assert.That(path, Is.EqualTo("root/dir1/dir2/file.txt"));
    }

    [Test]
    public void Navigate_ValidPath_ReturnsCorrectNode()
    {
        // Arrange
        var root = FileNode.CreateRoot("root");
        var dir1 = FileNode.CreateDirectory("dir1");
        var file = FileNode.CreateFile("file.txt", 100);
        root.AddChild(dir1);
        dir1.AddChild(file);

        // Act
        var found = root.Navigate("dir1/file.txt");

        // Assert
        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Name, Is.EqualTo("file.txt"));
    }

    [Test]
    public void Navigate_InvalidPath_ReturnsNull()
    {
        // Arrange
        var root = FileNode.CreateRoot("root");

        // Act
        var found = root.Navigate("nonexistent/path");

        // Assert
        Assert.That(found, Is.Null);
    }

    [Test]
    public void Exists_ValidPath_ReturnsTrue()
    {
        // Arrange
        var root = FileNode.CreateRoot("root");
        var dir = FileNode.CreateDirectory("dir");
        root.AddChild(dir);

        // Act
        var exists = root.Exists("dir");

        // Assert
        Assert.That(exists, Is.True);
    }

    [Test]
    public void Exists_InvalidPath_ReturnsFalse()
    {
        // Arrange
        var root = FileNode.CreateRoot("root");

        // Act
        var exists = root.Exists("nonexistent");

        // Assert
        Assert.That(exists, Is.False);
    }

    [Test]
    public void CreatedTime_IsSetOnCreation()
    {
        // Arrange
        var before = DateTime.Now;

        // Act
        var node = FileNode.CreateFile("test.txt", 100);
        var after = DateTime.Now;

        // Assert
        Assert.That(node.CreatedTime, Is.GreaterThanOrEqualTo(before));
        Assert.That(node.CreatedTime, Is.LessThanOrEqualTo(after));
    }

    [Test]
    public void ModifiedTime_UpdatesOnAddChild()
    {
        // Arrange
        var parent = FileNode.CreateDirectory("parent");
        var originalModifiedTime = parent.ModifiedTime;
        Thread.Sleep(10); // Ensure time difference

        // Act
        var child = FileNode.CreateFile("child.txt", 100);
        parent.AddChild(child);

        // Assert
        Assert.That(parent.ModifiedTime, Is.GreaterThan(originalModifiedTime));
    }
}
