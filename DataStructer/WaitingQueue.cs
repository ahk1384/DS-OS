using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using DS_OS.Exceptions;

namespace DS_OS.DataStructer;

public class WaitingQueue
{
    private readonly Queue<Pcb> _FQueue;
    private readonly Queue<Pcb> _RQueue;

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
            return false;
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
            return false;
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
        if (RemoveFromReadyLimit(target))
        {
            return RemoveFromWaitingFile(target);
        }
        return false;
    }

    private bool Remove(Pcb target,Queue<Pcb> queue)
    {
        bool found = false;
        int i = 0;
        int size = queue.Count;
        while (i < size)
        {
            Pcb v1 = queue.Dequeue();
            if (!v1.Pid.Equals(target.Pid))
            {
                queue.Enqueue(v1);
            }
            else
            {
                found = true;
            }
            i++;
        }

        if (!found)
        {
            throw new ProcessNotFoundException($"the {target.Pid} Not Found");
        }
        return found;
    }

    public bool isEmptyReadyLimit()
    {
        if (_RQueue.Count == 0)
        {
            return true;
        }
        return false;
    }

    public bool isEmptyFileWaiting()
    {
        if (_FQueue.Count == 0)
        {
            return true;
        }
        return false;
    }

    public bool isEmpty()
    {
        if (isEmptyFileWaiting())
        {
            return isEmptyReadyLimit();
        }

        return false;
    }

}