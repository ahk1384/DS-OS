using DS_OS.DataStructer;

namespace DS_OS.Engine.ProccesManager;

public interface IProcessManager
{
    bool Update();
    Task<bool> CheckFileExist(Pcb process);
    Task TimeOut(Pcb process);
    bool CheckReadyLimit();

    bool Config(int Prf);
}