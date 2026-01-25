namespace DS_OS.Engine.ProcessExcuter;

public interface IProcessExecutor
{
    bool Start();
    bool End();
    bool Stop();
    bool Continue();

}