using DS_OS.DataStructer;

namespace DS_OS.FileManager;

public class FileManager: IFileManager
{
    public bool CreateFile(string path, string name, long size = 0)
    {
        throw new NotImplementedException();
    }

    public bool CreateDirectory(string path, string name)
    {
        throw new NotImplementedException();
    }

    public bool Delete(string path)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string path)
    {
        throw new NotImplementedException();
    }

    public FileNode? GetNode(string path)
    {
        throw new NotImplementedException();
    }

    public FileNode? GetParent(string path)
    {
        throw new NotImplementedException();
    }

    public string GetFullPath(FileNode node)
    {
        throw new NotImplementedException();
    }

    public long GetDirectorySize(string path)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<FileNode> ListDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<FileNode> GetFiles(string path)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<FileNode> GetDirectories(string path)
    {
        throw new NotImplementedException();
    }

    public bool Move(string sourcePath, string destinationPath)
    {
        throw new NotImplementedException();
    }

    public bool Copy(string sourcePath, string destinationPath)
    {
        throw new NotImplementedException();
    }

    public bool Rename(string path, string newName)
    {
        throw new NotImplementedException();
    }

    public FileNode GetRoot()
    {
        throw new NotImplementedException();
    }

    public FileNode? Navigate(string path)
    {
        throw new NotImplementedException();
    }

    public string GetCurrentDirectory()
    {
        throw new NotImplementedException();
    }

    public bool ChangeDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public bool IsFile(string path)
    {
        throw new NotImplementedException();
    }

    public bool IsDirectory(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetCreatedTime(string path)
    {
        throw new NotImplementedException();
    }

    public DateTime GetModifiedTime(string path)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<FileNode> Search(string pattern, string searchPath = "/")
    {
        throw new NotImplementedException();
    }

    public IEnumerable<FileNode> FindByName(string name, string searchPath = "/")
    {
        throw new NotImplementedException();
    }

    public bool IsValidPath(string path)
    {
        throw new NotImplementedException();
    }

    public bool IsValidFileName(string name)
    {
        throw new NotImplementedException();
    }
}