using DS_OS.DataStructer;

namespace DS_OS.FileManager;

public interface IFileManager
{
    bool AddFile(FileNode file);
    bool RemoveFile(FileNode file);
    bool IsExist(string path);

}