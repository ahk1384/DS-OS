namespace DS_OS.Logger;

public class Logger : ILogger
{
    private string path = "E:\\Amir\\git hub repository work\\DS-OS\\DS-OS\\Long.txt";
    private string[] log;
    public bool Log(LongType message)
    {
        log.Append($"{log.Count()+1} | "+message.ToString());
        return true;
    }

    public bool Log(LongType[] messages)
    {
        foreach (LongType message in messages)
        {
            Log(message);
        }
        SaveLog();
        return true;
    }
    public bool SaveLog()
    {
        string[] descreption = ["--CYCLE SUMMARY--", "PID | RESULT "];
        File.AppendAllLines(path,descreption);
        File.AppendAllLines(path,log);
        return true;
    }

    public bool ClearLog()
    {
        File.WriteAllLines(path, [""]);
        return true;
    }

}