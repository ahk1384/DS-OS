using DS_OS.DataBaseManager;
using DS_OS.DataStructer;
using DS_OS.FileManager;

namespace DS_OS.Engine.ProccesManager;

public class ProcessManager(IDataBaseManager dataBaseManager, IFileManager fileManager) : IProcessManager
{
    private readonly IDataBaseManager _db = dataBaseManager;
    private readonly IFileManager _fm = fileManager;
    public int Prf { private get; set; }

    public bool Update()
    {
        return CheckWaiting().Result;
    }

    private async Task<bool> CheckWaiting()
    {
        for (int i = 0; i < _db.GetFileWaitingCount(); i++)
        {
            var process = _db.GetNextFileWaitingProcess();
            if (process is null) continue;
            if (process.NeedsFile)
            {
                if (_fm.Exists(process.FilePath))
                {
                    _db.MoveToReady(process);
                }
                else
                {
                    _db.MoveToWaitingFile(process);
                }
            }
            else
            {
                _db.MoveToReady(process);
            }
        }

        return true;
    }

    public bool CheckReadyLimit()
    {
        if (!_db.IsReadyFull())
        {
            return _db.MoveToReady(_db.GetNextReadyLimitProcess());
        }

        return false;
    }

    public bool Config(int Prf)
    {
        this.Prf = Prf;
        return true;
    }

    public async Task<bool> CheckFileExist(Pcb process)
    {
        if (process.NeedsFile)
        {
            if (!_fm.Exists(process.FilePath))
            {
                _db.MoveToWaitingFile(process);
                return false;
            }
        }

        return true;
    }

    public async Task TimeOut(Pcb pcb)
    {
        if (pcb != null)
        {
            pcb.Priority = pcb.Priority / (Prf * pcb.RunedQuantum + 1);
            while (!_db.IsReadyQueueEmpty())
            {
                for (int i = 0; i < _db.GetReadyLimitWaitingCount(); i++)
                {
                    if (!_db.IsReadyFull() && _db.PeekNextReadyLimit().Priority > pcb.Priority)
                    {
                        _db.MoveToReady(_db.GetNextReadyLimitProcess());
                    }
                }

                if (!_db.IsReadyFull())
                {
                    _db.MoveToReady(pcb);
                    break;
                }
            }
        }
    }
}