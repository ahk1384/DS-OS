using DS_OS.DataStructer;

namespace DS_OS.FileManager;

public class FileManager : IFileManager
{
    private readonly FileTree _fileTree;
    private string _currentDirectory;
    private readonly object _lock = new object();

    public FileManager()
    {
        _fileTree = new FileTree();
        _currentDirectory = "/";
    }

    #region Basic File/Directory Operations

    public bool CreateFile(string path, string name, long size = 0)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.CreateFile(path, name, size);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool CreateDirectory(string path, string name)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.CreateDirectory(path, name);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool Delete(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.Delete(path);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool Exists(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.Exists(path);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    #endregion

    #region File/Directory Information

    public FileNode? GetNode(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.FindNode(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public FileNode? GetParent(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.GetParent(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public string GetFullPath(FileNode node)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.GetFullPath(node);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }

    public long GetDirectorySize(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.GetDirectorySize(path);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    #endregion

    #region Directory Listing

    public IEnumerable<FileNode> ListDirectory(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.ListDirectory(path).ToList();
            }
            catch (Exception)
            {
                return Enumerable.Empty<FileNode>();
            }
        }
    }

    public IEnumerable<FileNode> GetFiles(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.GetFiles(path).ToList();
            }
            catch (Exception)
            {
                return Enumerable.Empty<FileNode>();
            }
        }
    }

    public IEnumerable<FileNode> GetDirectories(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.GetDirectories(path).ToList();
            }
            catch (Exception)
            {
                return Enumerable.Empty<FileNode>();
            }
        }
    }

    #endregion

    #region File/Directory Manipulation

    public bool Move(string sourcePath, string destinationPath)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.Move(sourcePath, destinationPath);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool Copy(string sourcePath, string destinationPath)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.Copy(sourcePath, destinationPath);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool Rename(string path, string newName)
    {
        lock (_lock)
        {
            try
            {
                if (!IsValidFileName(newName))
                    return false;

                var node = GetNode(path);
                if (node == null)
                    return false;

                var parent = node.Parent;
                if (parent == null)
                    return false; // Cannot rename root

                // Check if new name already exists in parent
                if (parent.Children.ContainsKey(newName))
                    return false;

                // Remove from parent with old name
                parent.RemoveChild(node.Name);

                // Update node name
                node.Name = newName;

                // Add back with new name
                return parent.AddChild(node);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    #endregion

    #region Navigation

    public FileNode GetRoot()
    {
        lock (_lock)
        {
            return _fileTree.Root;
        }
    }

    public FileNode? Navigate(string path)
    {
        lock (_lock)
        {
            try
            {
                return _fileTree.FindNode(path);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public string GetCurrentDirectory()
    {
        lock (_lock)
        {
            return _currentDirectory;
        }
    }

    public bool ChangeDirectory(string path)
    {
        lock (_lock)
        {
            try
            {
                var node = _fileTree.FindNode(path);
                if (node != null && node.IsDirectory)
                {
                    _currentDirectory = node.GetFullPath();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    #endregion

    #region File Properties

    public bool IsFile(string path)
    {
        lock (_lock)
        {
            try
            {
                var node = GetNode(path);
                return node?.IsFile ?? false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool IsDirectory(string path)
    {
        lock (_lock)
        {
            try
            {
                var node = GetNode(path);
                return node?.IsDirectory ?? false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public DateTime GetCreatedTime(string path)
    {
        lock (_lock)
        {
            try
            {
                var node = GetNode(path);
                return node?.CreatedTime ?? DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }

    public DateTime GetModifiedTime(string path)
    {
        lock (_lock)
        {
            try
            {
                var node = GetNode(path);
                return node?.ModifiedTime ?? DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }

    #endregion

    #region Search Operations

    public IEnumerable<FileNode> Search(string pattern, string searchPath = "/")
    {
        lock (_lock)
        {
            try
            {
                var startNode = _fileTree.FindNode(searchPath);
                if (startNode == null)
                    return Enumerable.Empty<FileNode>();

                var results = new List<FileNode>();
                SearchRecursive(startNode, pattern, results);
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<FileNode>();
            }
        }
    }

    private void SearchRecursive(FileNode node, string pattern, List<FileNode> results)
    {
        if (node == null)
            return;

        // Simple pattern matching (contains)
        if (node.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(node);
        }

        // Search in children if directory
        if (node.IsDirectory && node.Children != null)
        {
            foreach (var child in node.Children.Values)
            {
                SearchRecursive(child, pattern, results);
            }
        }
    }

    public IEnumerable<FileNode> FindByName(string name, string searchPath = "/")
    {
        lock (_lock)
        {
            try
            {
                var startNode = _fileTree.FindNode(searchPath);
                if (startNode == null)
                    return Enumerable.Empty<FileNode>();

                var results = new List<FileNode>();
                FindByNameRecursive(startNode, name, results);
                return results;
            }
            catch (Exception)
            {
                return Enumerable.Empty<FileNode>();
            }
        }
    }

    private void FindByNameRecursive(FileNode node, string name, List<FileNode> results)
    {
        if (node == null)
            return;

        // Exact name match
        if (node.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            results.Add(node);
        }

        // Search in children if directory
        if (node.IsDirectory && node.Children != null)
        {
            foreach (var child in node.Children.Values)
            {
                FindByNameRecursive(child, name, results);
            }
        }
    }

    #endregion

    #region Validation

    public bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        // Check for invalid characters
        char[] invalidChars = Path.GetInvalidPathChars();
        if (path.IndexOfAny(invalidChars) >= 0)
            return false;

        // Path should start with /
        if (!path.StartsWith("/"))
            return false;

        return true;
    }

    public bool IsValidFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        // Check for invalid characters in filename
        char[] invalidChars = Path.GetInvalidFileNameChars();
        if (name.IndexOfAny(invalidChars) >= 0)
            return false;

        // Check for reserved names (Windows compatibility)
        string[] reservedNames = { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", 
                                   "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", 
                                   "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };
        
        string upperName = name.ToUpperInvariant();
        if (reservedNames.Contains(upperName))
            return false;

        // Name should not contain path separators
        if (name.Contains('/') || name.Contains('\\'))
            return false;

        return true;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Clear all files and directories except root
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            try
            {
                var root = _fileTree.Root;
                if (root.Children != null)
                {
                    var childrenToRemove = root.Children.Keys.ToList();
                    foreach (var childName in childrenToRemove)
                    {
                        root.RemoveChild(childName);
                    }
                }
                _currentDirectory = "/";
            }
            catch (Exception)
            {
                // Ignore errors during clear
            }
        }
    }

    /// <summary>
    /// Get total number of files and directories in the file system
    /// </summary>
    public int GetTotalNodeCount()
    {
        lock (_lock)
        {
            try
            {
                return CountNodesRecursive(_fileTree.Root);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    private int CountNodesRecursive(FileNode node)
    {
        if (node == null)
            return 0;

        int count = 1; // Count current node

        if (node.IsDirectory && node.Children != null)
        {
            foreach (var child in node.Children.Values)
            {
                count += CountNodesRecursive(child);
            }
        }

        return count;
    }

    #endregion
}