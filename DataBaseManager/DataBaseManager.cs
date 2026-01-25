using System.Net.NetworkInformation;
using DS_OS.DataStructer;
using DS_OS.FileManager;

namespace DS_OS.DataBaseManager;

public class DataBaseManager(IFileManager fileManager) : IDataBaseManager
{
    private readonly WaitingQueue _waiting = new WaitingQueue();
    private readonly ReadyPriorityQueue _ready = new ReadyPriorityQueue();
    private readonly ProcessTree _processTree = new ProcessTree(Pcb.Create(0, null, 0, 0));
    private readonly IFileManager _fileManager = fileManager;

    public bool AddProcess(Pcb process)
    {
        _processTree.AddProcess(process.Parent,process);
        if (process.NeedsFile)
        {
            if (_fileManager.Exists(process.FilePath))
            {
                 return _ready.Enqueue(process);
            }
            else
            {
                return _waiting.EnqueueFileWating(process);
            }
        }
        else
        {
            return _ready.Enqueue(process);
        }
    }

    public bool DeleteProcess(Pcb process)
    {
        throw new NotImplementedException();
    }

    public Pcb GetPcb(int pid)
    {
        return _processTree.GetProcess(pid);
    }
    
}