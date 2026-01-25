using DS_OS.Exceptions;

namespace DS_OS.DataStructer;

public class FileTree
{
    private readonly FileNode Root = FileNode.CreateRoot("/");

    private bool Create(string path, string name,bool IsFile=false)
    {
        path += "/name";
        var paths = path.Split("/");
        var current = Root;

        current = Find(paths, current);
        if (current.AddChild(new FileNode(name, IsFile))) return true;

        return false;
    }
    public bool CreateFile(string path, string name)
    {
        if (Create(path,name,true)) return true;
        return false;
    }
    public bool CreateDirectory(string path, string name)
    {
        if (Create(path,name,false)) return true;
        return false;
    }
    public bool Delete(string path)
    {
        var paths = path.Split("/");
        var current = Root;
        current = Find(paths, current);
        if (current.RemoveChild(paths[^1])) return true;

        return false;
    }

    private FileNode? Find(string[] paths, FileNode? current)
    {
        foreach (var node in paths[new Range(0, paths.Length - 2)])
            if (current is { IsDirectory: true } && current.Children.ContainsKey(node))
            {
                current = current.Children.GetValueOrDefault(node);
            }
            else
            {
                throw new InvalidPathException();
                return null;
            }

        return current;
    }

    public bool IsExist(string path)
    {
        string[] paths = path.Split("/");
        try
        {
            if (Find(paths, Root) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            return false;
        }
    }


}