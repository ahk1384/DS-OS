using DS_OS.DataStructer;

namespace DS_OS.DataBaseManager;

public interface IDataBaseManager 
{
    bool AddProcess(Pcb process);
    bool DeleteProcess(Pcb process);

    Pcb GetPcb(int parentPid);
}