using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using DS_OS.Exceptions;

namespace DS_OS.DataStructer;

public class WaitingQueue
{
    private readonly Queue<Pcb> _FQueue;
    private readonly Queue<Pcb> _RQueue;

    public WaitingQueue()
    {
        _FQueue = new Queue<Pcb>();
        _RQueue = new Queue<Pcb>();
    }

    public int FileWaitingCount => _FQueue.Count;
    public int ReadyLimitCount => _RQueue.Count;
    public int TotalCount => _FQueue.Count + _RQueue.Count;

    public bool EnqueueFileWating(Pcb pcb)
    {
        if (pcb.State.Equals(State.Waiting) && pcb.WaitReason.Equals(WaitReason.File))
        {
            _FQueue.Enqueue(pcb);
            return true;
        }
        else
        {
            throw new InvalidStateException();
        }
    }

    public bool EnqueueReadyLimit(Pcb pcb)
    {
        if (pcb.State.Equals(State.Waiting) && pcb.WaitReason.Equals(WaitReason.ReadyLimit))
        {
            _RQueue.Enqueue(pcb);
            return true; 
        }
        else
        {
            throw new InvalidStateException();
        }
    }

    public Pcb DequeueReadyLimit()
    {
        return _RQueue.Dequeue();
    }

    public Pcb DequeueFileWaiting()
    {
        return _FQueue.Dequeue();
    }

    public bool RemoveFromWaitingFile(Pcb target)
    {
        return Remove(target,_FQueue);
    }
    
    public bool RemoveFromReadyLimit(Pcb target)
    {
        return Remove(target,_RQueue);
    }

    public bool RemoveFromAll(Pcb target)
    {
        bool removedFromReady = false;
        bool removedFromFile = false;

        try
        {
            removedFromReady = RemoveFromReadyLimit(target);
        }
        catch (ProcessNotFoundException)
        {
            // Process not in ready limit queue
        }

        try
        {
            removedFromFile = RemoveFromWaitingFile(target);
        }
        catch (ProcessNotFoundException)
        {
            // Process not in file waiting queue
        }

        return removedFromReady || removedFromFile;
    }

    private bool Remove(Pcb target, Queue<Pcb> queue)
    {
        bool found = false;
        int size = queue.Count;
        
        for (int i = 0; i < size; i++)
        {
            Pcb current = queue.Dequeue();
            if (!current.Pid.Equals(target.Pid))
            {
                queue.Enqueue(current);
            }
            else
            {
                found = true;
            }
        }

        if (!found)
        {
            throw new ProcessNotFoundException($"Process {target.Pid} not found");
        }
        
        return found;
    }

    public bool IsEmptyReadyLimit()
    {
        return _RQueue.Count == 0;
    }

    public bool IsEmptyFileWaiting()
    {
        return _FQueue.Count == 0;
    }

    public bool IsEmpty()
    {
        return IsEmptyFileWaiting() && IsEmptyReadyLimit();
    }

    public Pcb PeakWaitingFile()
    {
        return _FQueue.Peek();
    }

    public Pcb PeakReadyLimit()
    {
        return _RQueue.Peek();
    }

    public void Clear()
    {
        _FQueue.Clear();
        _RQueue.Clear();
    }
}