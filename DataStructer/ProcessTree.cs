using System.Collections.Generic;
using DS_OS.Exceptions;

namespace DS_OS.DataStructer
{
    internal class ProcessTree
    {
        private readonly Dictionary<int, Pcb> _processMap = new();
        public Pcb Root { get; private set; }

        public ProcessTree(Pcb initProcess)
        {
            Root = initProcess ?? throw new InvalidParentException("Root process cannot be null.");
            _processMap.Add(Root.Pid, Root);
        }

        public void AddProcess(Pcb parent, Pcb child)
        {
            if (!_processMap.ContainsKey(parent.Pid))
                throw new ProcessNotFoundException($"Parent PID {parent.Pid} not found.");

            parent.Children ??= new Dictionary<int, Pcb>();
            parent.Children.Add(child.Pid,child);
            child.Parent = parent;
            _processMap.Add(child.Pid, child);
        }

        public Pcb GetProcess(int pid)
        {
            if (_processMap.TryGetValue(pid, out var pcb))
                return pcb;
            
            throw new ProcessNotFoundException($"PID {pid} not found.");
        }

        public void RemoveProcess(int pid)
        {
            if (!_processMap.TryGetValue(pid, out var process))
            {
                throw new ProcessNotFoundException($"PID {pid} not found.");
            }

            var childPids = new List<int>(process.Children.Keys);
            foreach (var childPid in childPids)
            {
                if (_processMap.TryGetValue(childPid, out var child))
                {
                    child.FinishTime = process.FinishTime;
                }

                RemoveProcess(childPid);
            }
            //Process Remover should be in here 
            process.Parent?.Children.Remove(pid);
            _processMap.Remove(pid);
        }




    }
}
