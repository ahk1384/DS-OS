using Logs;
using System.IO.Pipes;
using System.Text;
namespace DS_OS.Logger;

public class Logger : ILogger
{
    private string path = "E:\\Amir\\git hub repository work\\DS-OS\\DS-OS\\Long.txt";
    private List<string> log = new List<string>();
    
    public bool Log(string message)
    {
        if (message == nameof(LongType.SHUTDOWN))
        {
            SendLog("shutdown");
        }
        else
        {
            SendLog(message);
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
        return true;
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
    

    static void SendLog(string message)
    {
        
        using var client = new NamedPipeClientStream(".", "MyLogPipe", PipeDirection.Out);
        client.Connect(1000);
        using var writer = new StreamWriter(client);
        writer.Write(message);
        writer.Flush();

       
            
    }

}