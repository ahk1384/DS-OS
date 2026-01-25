namespace DS_OS.DataStructer;

public class FileNode
{
    public string Name { get; set; }
    public int Hash { get; private set; }
    public FileNode? Parent { get; set; } = null;
    public bool IsFile { get; set; }
    public bool IsDirectory => !IsFile;
    public Dictionary<string, FileNode> Children { get; set; }
    public bool IsRoot { get; set; }
    public string FullPath => GetFullPath();
    public DateTime CreatedTime { get; set; }
    public DateTime ModifiedTime { get; set; }
    public long Size { get; set; } // File size in bytes, 0 for directories

    public FileNode(string name, bool isFile)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        Name = name;
        Hash = name.GetHashCode();
        IsFile = isFile;
        Children = new Dictionary<string, FileNode>();
        CreatedTime = DateTime.Now;
        ModifiedTime = DateTime.Now;
        Size = 0;
        IsRoot = false;
    }

    private FileNode(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty", nameof(name));

        Name = name;
        Hash = name.GetHashCode();
        IsFile = false; // Root is always a directory
        Children = new Dictionary<string, FileNode>();
        CreatedTime = DateTime.Now;
        ModifiedTime = DateTime.Now;
        Size = 0;
        IsRoot = true;
    }

    public static FileNode CreateRoot(string name)
    {
        return new FileNode(name);
    }

    public static FileNode CreateFile(string name, long size = 0)
    {
        return new FileNode(name, true) { Size = size };
    }

    public static FileNode CreateDirectory(string name)
    {
        return new FileNode(name, false);
    }

    public bool AddChild(FileNode node)
    {
        if (node == null)
            throw new ArgumentNullException(nameof(node));

        if (IsFile)
            throw new InvalidOperationException("Cannot add children to a file");

        if (Children.ContainsKey(node.Name))
            return false; // Child with same name already exists

        node.Parent = this;
        Children.Add(node.Name, node);
        ModifiedTime = DateTime.Now;
        return true;
    }

    public bool RemoveChild(string name)
    {
        if (IsFile)
            throw new InvalidOperationException("Cannot remove children from a file");

        if (Children.Remove(name))
        {
            ModifiedTime = DateTime.Now;
            return true;
        }
        return false;
    }

    public FileNode? FindChild(string name)
    {
        return Children.TryGetValue(name, out FileNode? child) ? child : null;
    }

    public IEnumerable<FileNode> GetFiles()
    {
        return Children.Values.Where(child => child.IsFile);
    }

    public IEnumerable<FileNode> GetDirectories()
    {
        return Children.Values.Where(child => child.IsDirectory);
    }

    public string GetFullPath()
    {
        if (IsRoot || Parent == null)
            return Name;

        var pathParts = new List<string>();
        var current = this;

        while (current != null && !current.IsRoot)
        {
            pathParts.Add(current.Name);
            current = current.Parent;
        }

        if (current?.IsRoot == true)
            pathParts.Add(current.Name);

        pathParts.Reverse();
        return string.Join("/", pathParts);
    }

    public bool Exists(string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = this;

        foreach (var part in parts)
        {
            if (!current.Children.TryGetValue(part, out current))
                return false;
        }

        return true;
    }

    public FileNode? Navigate(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/")
            return this;

        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var current = this;

        foreach (var part in parts)
        {
            if (!current.Children.TryGetValue(part, out current))
                return null;
        }

        return current;
    }

    public override string ToString()
    {
        return $"{Name} ({(IsFile ? "File" : "Directory")})";
    }
}