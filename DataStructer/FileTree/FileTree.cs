using DS_OS.Exceptions;

namespace DS_OS.DataStructer;

public class FileTree
{
    private readonly FileNode _root = FileNode.CreateRoot("/");

    public FileNode Root => _root;

    public bool CreateFile(string path, string name, long size = 0)
    {
        try
        {
            var parentNode = FindNode(path);
            if (parentNode == null || parentNode.IsFile)
                return false;

            var fileNode = FileNode.CreateFile(name, size);
            return parentNode.AddChild(fileNode);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateDirectory(string path, string name)
    {
        try
        {
            var parentNode = FindNode(path);
            if (parentNode == null || parentNode.IsFile)
                return false;

            var dirNode = FileNode.CreateDirectory(name);
            return parentNode.AddChild(dirNode);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Delete(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return false;

            var pathParts = SplitPath(path);
            if (pathParts.Length == 0)
                return false;

            string parentPath;
            if (pathParts.Length == 1)
                parentPath = "/";
            else
                parentPath = "/" + string.Join("/", pathParts.Take(pathParts.Length - 1));
                
            var fileName = pathParts.Last();

            var parentNode = parentPath == "/" ? _root : FindNode(parentPath);
            if (parentNode == null || parentNode.IsFile)
                return false;

            return parentNode.RemoveChild(fileName);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Exists(string path)
    {
        try
        {
            return FindNode(path) != null;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public FileNode? FindNode(string path)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return _root;

            return _root.Navigate(path);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public FileNode? GetParent(string path)
    {
        try
        {
            var pathParts = SplitPath(path);
            if (pathParts.Length <= 1)
                return _root;

            var parentPath = "/" + string.Join("/", pathParts.Take(pathParts.Length - 1));
            return FindNode(parentPath);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public IEnumerable<FileNode> ListDirectory(string path)
    {
        var node = FindNode(path);
        if (node?.IsDirectory == true)
        {
            return node.Children.Values;
        }
        return Enumerable.Empty<FileNode>();
    }

    public IEnumerable<FileNode> GetFiles(string path)
    {
        var node = FindNode(path);
        if (node?.IsDirectory == true)
        {
            return node.GetFiles();
        }
        return Enumerable.Empty<FileNode>();
    }

    public IEnumerable<FileNode> GetDirectories(string path)
    {
        var node = FindNode(path);
        if (node?.IsDirectory == true)
        {
            return node.GetDirectories();
        }
        return Enumerable.Empty<FileNode>();
    }

    public bool Move(string sourcePath, string destinationPath)
    {
        try
        {
            var sourceNode = FindNode(sourcePath);
            if (sourceNode == null || sourceNode.IsRoot)
                return false;

            var destParent = FindParentNode(destinationPath);
            if (destParent == null || destParent.IsFile)
                return false;

            if (sourceNode.Parent?.RemoveChild(sourceNode.Name) == true)
            {
                var pathParts = SplitPath(destinationPath);
                var newName = pathParts.Last();
                sourceNode.Name = newName;
                
                return destParent.AddChild(sourceNode);
            }

            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Copy(string sourcePath, string destinationPath)
    {
        try
        {
            var sourceNode = FindNode(sourcePath);
            if (sourceNode == null)
                return false;

            var destParent = FindParentNode(destinationPath);
            if (destParent == null || destParent.IsFile)
                return false;

            var pathParts = SplitPath(destinationPath);
            var newName = pathParts.Last();

            FileNode newNode;
            if (sourceNode.IsFile)
            {
                newNode = FileNode.CreateFile(newName, sourceNode.Size);
            }
            else
            {
                newNode = FileNode.CreateDirectory(newName);
                CopyChildren(sourceNode, newNode);
            }

            return destParent.AddChild(newNode);
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void CopyChildren(FileNode source, FileNode destination)
    {
        foreach (var child in source.Children.Values)
        {
            FileNode newChild;
            if (child.IsFile)
            {
                newChild = FileNode.CreateFile(child.Name, child.Size);
            }
            else
            {
                newChild = FileNode.CreateDirectory(child.Name);
                CopyChildren(child, newChild);
            }
            destination.AddChild(newChild);
        }
    }

    private FileNode? FindParentNode(string path)
    {
        var pathParts = SplitPath(path);
        if (pathParts.Length == 0)
            return _root;

        var parentPath = "/" + string.Join("/", pathParts.Take(pathParts.Length - 1));
        
        // Handle root case
        if (parentPath == "/")
            return _root;
            
        return FindNode(parentPath);
    }

    private string[] SplitPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return Array.Empty<string>();

        return path.Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    public string GetFullPath(FileNode node)
    {
        return node.GetFullPath();
    }

    public long GetDirectorySize(string path)
    {
        var node = FindNode(path);
        if (node == null)
            return 0;

        return CalculateSize(node);
    }

    private long CalculateSize(FileNode node)
    {
        if (node.IsFile)
            return node.Size;

        long totalSize = 0;
        foreach (var child in node.Children.Values)
        {
            totalSize += CalculateSize(child);
        }
        return totalSize;
    }
}