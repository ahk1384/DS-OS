namespace DS_OS.Engine.ProcessExcuter;

public interface IProcessExecutor
{
    void Start();
    bool End();

    bool Config(int executionPerCycle, int quantumSize);
}