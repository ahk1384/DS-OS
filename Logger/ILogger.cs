using DS_OS.DataStructer;

namespace DS_OS.Logger;

public interface ILogger
{
    bool Log(string message);
    bool Log(string[] messages);
    bool SaveLog();
    bool ClearLog();
    void Log(IEnumerable<FileNode> getFiles);
}