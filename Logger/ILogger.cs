namespace DS_OS.Logger;

public interface ILogger
{
    bool Log(LongType message);
    bool Log(LongType[] messages);
    bool SaveLog();
    bool ClearLog();
}