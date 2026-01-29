using DS_OS.DataStructer;

namespace DS_OS.DataBaseManager;

public interface IDataBaseManager 
{
    // Process Management
    bool AddProcess(Pcb process);
    bool DeleteProcess(Pcb process);
    bool DeleteProcess(int pid);
    Pcb GetPcb(int pid);
    bool ProcessExists(int pid);
    
    // Ready Queue Operations
    Pcb GetNextReadyProcess();
    bool MoveToReady(Pcb process);
    int GetReadyQueueCount();
    bool IsReadyQueueEmpty();
    Pcb PeekNextReady();

    bool IsReadyFull();
    // Waiting Queue Operations
    bool MoveToWaitingFile(Pcb process);
    bool MoveToWaitingReadyLimit(Pcb process);
    Pcb GetNextFileWaitingProcess();
    Pcb GetNextReadyLimitProcess();
    bool RemoveFromWaiting(Pcb process);
    int GetFileWaitingCount();
    int GetReadyLimitWaitingCount();
    bool IsWaitingQueueEmpty();
    
    Pcb PeekNextReadyLimit();
    // Process Tree Operations
    Pcb GetRootProcess();
    IEnumerable<Pcb> GetChildren(int pid);
    IEnumerable<Pcb> GetAllProcesses();
    int GetTotalProcessCount();
    
    // State Management
    bool UpdateProcessState(int pid, State newState);
    IEnumerable<Pcb> GetProcessesByState(State state);
    
    // Utility Operations
    void Clear();
    bool CanScheduleProcess(Pcb process);
    bool Config(int ReadyLimit);
}