using DS_OS.DataStructer;

namespace DS_OS.FileManager;

public interface IFileManager
{
    // Basic File/Directory Operations
    bool CreateFile(string path, string name, long size = 0);
    bool CreateDirectory(string path, string name);
    bool Delete(string path);
    bool Exists(string path);
    
    // File/Directory Information
    FileNode? GetNode(string path);
    FileNode? GetParent(string path);
    string GetFullPath(FileNode node);
    long GetDirectorySize(string path);
    
    // Directory Listing
    IEnumerable<FileNode> ListDirectory(string path);
    IEnumerable<FileNode> GetFiles(string path);
    IEnumerable<FileNode> GetDirectories(string path);
    IEnumerable<FileNode> GetAllFilesRecursive(string path);
    
    // File/Directory Manipulation
    bool Move(string sourcePath, string destinationPath);
    bool Copy(string sourcePath, string destinationPath);
    bool Rename(string path, string newName);
    
    // Navigation
    FileNode GetRoot();
    FileNode? Navigate(string path);
    string GetCurrentDirectory();
    bool ChangeDirectory(string path);
    
    // File Properties
    bool IsFile(string path);
    bool IsDirectory(string path);
    DateTime GetCreatedTime(string path);
    DateTime GetModifiedTime(string path);
    
    // Search Operations
    IEnumerable<FileNode> Search(string pattern, string searchPath = "/");
    IEnumerable<FileNode> FindByName(string name, string searchPath = "/");
    
    // Validation
    bool IsValidPath(string path);
    bool IsValidFileName(string name);
}