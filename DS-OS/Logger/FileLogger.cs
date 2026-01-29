using System.IO.Pipes;
using DS_OS.DataStructer;

namespace DS_OS.Logger;

public class FileLogger : ILogger
{
    private string path = "E:\\Amir\\git hub repository work\\DS-OS\\DS-OS\\FileLog.txt";
    private List<string> log = new List<string>();

    public bool Log(string message)
    {
       
        log.Add(message);
        return true;
    }

    public bool Log(string[] messages)
    {
        foreach (string message in messages)
        {
            Log(message);
        }
        SaveLog();
        return true;
    }
    public bool SaveLog()
    {
        File.AppendAllLines(path, log);
        log.Clear();
        return true;
    }

    public bool ClearLog()
    {
        File.WriteAllText(path, string.Empty);
        return true;
    }

    public void Log(IEnumerable<FileNode> Files)
    {
        foreach (FileNode file in Files)
        {
            Log(file.FullPath+"\n");
        }
        SaveLog();
    }
}