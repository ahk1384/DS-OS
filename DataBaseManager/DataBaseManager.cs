using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using DS_OS.DataStructer;
using DS_OS.Exceptions;
using DS_OS.FileManager;
using DS_OS.Logger;

namespace DS_OS.DataBaseManager;

public class DataBaseManager : IDataBaseManager
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly WaitingQueue _waiting;
    private readonly ReadyPriorityQueue _ready;
    private readonly ProcessTree _processTree;
    private readonly IFileManager _fileManager;
    private readonly ILogger _logger;
    public DataBaseManager(IFileManager fileManager, ILogger logger)
    {
        _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        _logger = logger;
        _waiting = new WaitingQueue();
        _ready = new ReadyPriorityQueue();

        var initProcess = Pcb.Create(0, null!, 0, int.MaxValue);
        initProcess.State = State.Running;
        _processTree = new ProcessTree(initProcess);
    }

    #region Process Management

    public bool AddProcess(Pcb process)
    {
        _semaphore.Wait();
        try
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));

            _processTree.AddProcess(process.Parent, process);

            if (process.NeedsFile)
            {
                if (_fileManager.Exists(process.FilePath))
                {
                    process.State = State.Ready;
                    return _ready.Enqueue(process);
                }
                else
                {
                    process.State = State.Waiting;
                    process.WaitReason = WaitReason.File;
                    return _waiting.EnqueueFileWating(process);
                }
            }
            else
            {
                process.State = State.Ready;
                return _ready.Enqueue(process);
            }
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool DeleteProcess(Pcb process)
    {
        if (process == null)
            return false;

        return DeleteProcess(process.Pid);
    }

    public bool DeleteProcess(int pid)
    {
        _semaphore.Wait();
        try
        {
            var process = GetPcbInternal(pid);

            if (pid == 0)
                return false;

            RemoveFromAllQueues(process);
            _processTree.RemoveProcess(pid);

            return true;
        }
        catch (ProcessNotFoundException)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Pcb GetPcb(int pid)
    {
        _semaphore.Wait();
        try
        {
            return GetPcbInternal(pid);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private Pcb GetPcbInternal(int pid)
    {
        return _processTree.GetProcess(pid);
    }

    public bool ProcessExists(int pid)
    {
        _semaphore.Wait();
        try
        {
            _processTree.GetProcess(pid);
            return true;
        }
        catch (ProcessNotFoundException)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    #endregion

    #region Ready Queue Operations

    public Pcb GetNextReadyProcess()
    {
        _semaphore.Wait();
        try
        {
            if (_ready.IsEmpty)
                throw new InvalidOperationException("No ready processes available");

            var process = _ready.Dequeue();
            process.State = State.Running;
            return process;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool MoveToReady(Pcb process)
    {
        _semaphore.Wait();
        try
        {
            if (process == null)
                return false;

            _waiting.RemoveFromAll(process);
            if (_ready.IsFull())
            {
                process.State = State.Waiting;
                process.WaitReason = WaitReason.ReadyLimit;
                return _waiting.EnqueueReadyLimit(process);
            }
            else
            {
                process.State = State.Ready;
                return _ready.Enqueue(process);
            }
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int GetReadyQueueCount()
    {
        _semaphore.Wait();
        try
        {
            return _ready.Count;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsReadyQueueEmpty()
    {
        _semaphore.Wait();
        try
        {
            return _ready.IsEmpty;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Pcb PeekNextReady()
    {
        _semaphore.Wait();
        try
        {
            if (_ready.IsEmpty)
                throw new InvalidOperationException("No ready processes available");

            return _ready.Peek();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool SetReadyQueueMaxSize(int maxSize)
    {
        _semaphore.Wait();
        try
        {
            _ready.MaxSize = maxSize;
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsReadyFull()
    {
        _semaphore.Wait();
        try
        {
            return _ready.IsFull();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion

    #region Waiting Queue Operations

    public bool MoveToWaitingFile(Pcb process)
    {
        _semaphore.Wait();
        try
        {
            if (process == null)
                return false;

            _ready.Remove(process);

            process.State = State.Waiting;
            process.WaitReason = WaitReason.File;
            _logger.Log(LongType.WATING_FILE);
            return _waiting.EnqueueFileWating(process);
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool MoveToWaitingReadyLimit(Pcb process)
    {
        _semaphore.Wait();
        try
        {
            if (process == null)
                return false;

            _ready.Remove(process);

            process.State = State.Waiting;
            process.WaitReason = WaitReason.ReadyLimit;
            _logger.Log(LongType.WATING_READY_LIMT);
            return _waiting.EnqueueReadyLimit(process);
        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Pcb GetNextFileWaitingProcess()
    {
        _semaphore.Wait();
        try
        {
            if (_waiting.IsEmptyFileWaiting())
                throw new InvalidOperationException("No file waiting processes available");

            return _waiting.DequeueFileWaiting();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Pcb GetNextReadyLimitProcess()
    {
        _semaphore.Wait();
        try
        {
            if (_waiting.IsEmptyReadyLimit())
                throw new InvalidOperationException("No ready limit waiting processes available");

            return _waiting.DequeueReadyLimit();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool RemoveFromWaiting(Pcb process)
    {
        _semaphore.Wait();
        try
        {
            if (process == null)
                return false;

            return _waiting.RemoveFromAll(process);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int GetFileWaitingCount()
    {
        _semaphore.Wait();
        try
        {
            return _waiting.FileWaitingCount;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int GetReadyLimitWaitingCount()
    {
        _semaphore.Wait();
        try
        {
            return _waiting.ReadyLimitCount;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsWaitingQueueEmpty()
    {
        _semaphore.Wait();
        try
        {
            return _waiting.IsEmpty();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Pcb PeekNextReadyLimit()
    {
        _semaphore.Wait();
        try
        {
            return _waiting.PeakReadyLimit();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion

    #region Process Tree Operations

    public Pcb GetRootProcess()
    {
        _semaphore.Wait();
        try
        {
            return _processTree.Root;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IEnumerable<Pcb> GetChildren(int pid)
    {
        _semaphore.Wait();
        try
        {
            var process = GetPcbInternal(pid);
            return process.Children?.Values ?? Enumerable.Empty<Pcb>();
        }
        catch (ProcessNotFoundException)
        {
            return Enumerable.Empty<Pcb>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IEnumerable<Pcb> GetAllProcesses()
    {
        _semaphore.Wait();
        try
        {
            var allProcesses = new List<Pcb>();
            CollectAllProcesses(_processTree.Root, allProcesses);
            return allProcesses;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public int GetTotalProcessCount()
    {
        _semaphore.Wait();
        try
        {
            return GetAllProcesses().Count();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void CollectAllProcesses(Pcb process, List<Pcb> collection)
    {
        if (process == null)
            return;

        collection.Add(process);

        if (process.Children != null)
        {
            foreach (var child in process.Children.Values)
            {
                CollectAllProcesses(child, collection);
            }
        }
    }

    public Pcb PeekNextWaiting()
    {
        _semaphore.Wait();
        try
        {
            if (_waiting.IsEmptyFileWaiting())
                throw new InvalidOperationException("No ready processes available");

            return _waiting.PeakWaitingFile();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    #endregion

    #region State Management

    public bool UpdateProcessState(int pid, State newState)
    {
        _semaphore.Wait();
        try
        {
            var process = GetPcbInternal(pid);
            process.State = newState;
            return true;
        }
        catch (ProcessNotFoundException)
        {
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public IEnumerable<Pcb> GetProcessesByState(State state)
    {
        _semaphore.Wait();
        try
        {
            return GetAllProcesses().Where(p => p.State == state).ToList();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    #endregion

    #region Utility Operations

    public void Clear()
    {
        _semaphore.Wait();
        try
        {
            _waiting.Clear();
            _ready.Clear();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool CanScheduleProcess(Pcb process)
    {
        _semaphore.Wait();
        try
        {
            if (process == null)
                return false;

            if (process.NeedsFile)
            {
                return _fileManager.Exists(process.FilePath);
            }

            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool Config(int ReadyLimit)
    {
        _semaphore.Wait();
        try
        {
            _ready.MaxSize = ReadyLimit;
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void RemoveFromAllQueues(Pcb process)
    {
        try
        {
            _ready.Remove(process);
        }
        catch
        {
        }

        try
        {
            _waiting.RemoveFromAll(process);
        }
        catch
        {
        }
    }

    #endregion
}