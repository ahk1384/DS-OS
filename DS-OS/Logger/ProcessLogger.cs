using Logs;
using System.IO.Pipes;
using System.Text;
using DS_OS.DataStructer;

namespace DS_OS.Logger;

public class ProcessLogger : ILogger
{
    private string path = "E:\\Amir\\git hub repository work\\DS-OS\\DS-OS\\Long.txt";
    private List<string> log = new List<string>();
    private ILogger _loggerImplementation;

    public bool Log(string message)
    {
        if (message == nameof(LongType.SHUTDOWN))
        {
            SendLog("shutdown");
        }
        else
        {
            log.Add(message);
        }

        return true;
    }

    public bool Log(string[] messages)
    {
        foreach (string message in messages)
        {
            Log(message);
        }
        ShowLog();
        return true;
    }

    private void ShowLog()
    {
        string res = "";
        foreach (string message in log)
        {
            res += message + "\n";
        }

        SaveLog();
        SendLog(res);
    }

    public bool SaveLog()
    {
        string[] descreption = ["--CYCLE SUMMARY--", "PID | RESULT "];
        File.AppendAllLines(path, descreption);
        File.AppendAllLines(path, log);
        log.Clear();
        return true;
    }

    public bool ClearLog()
    {
        File.WriteAllText(path, string.Empty);
        return true;
    }

    public void Log(IEnumerable<FileNode> getFiles)
    {
        throw new NotImplementedException();
    }


    static void SendLog(string message)
    {
        
        using var client = new NamedPipeClientStream(".", "MyLogPipe", PipeDirection.Out);
        client.Connect(1000);
        using var writer = new StreamWriter(client);
        writer.Write(message);
        writer.Flush();

       
            
    }

}