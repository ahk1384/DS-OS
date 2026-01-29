using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DS_OS.Exceptions;

namespace DS_OS.DataStructer
{
    public class Pcb
    {
        public int Pid { get; private set; }
        public string Name { get; private set; }
        public int  Priority { get; set; }
        public int  RemaningTime { get; set; }
        public State State { get; set; }
        public bool NeedsFile { get; set; }
        public string FilePath { get; set; } = null;
        public WaitReason WaitReason { get; set; }      
        public Pcb Parent { get; set; }
        public Dictionary<int ,Pcb> Children { get; set; }
        public DateTime EntryTime { get; set; }
        public int StartTime { get; set; } = -1;
        public int FinishTime { get; set; }
        public int RunedQuantum { get; set; } = 0;

        private Pcb(int pid, Pcb parent, int priority, int remaningTime)
        {
            this.Pid = pid;
            this.Name = $"Process_{pid}";
            Parent = parent;
            this.Priority = priority;
            this.RemaningTime = remaningTime;
            State = State.New;
            Children = new Dictionary<int, Pcb>();
            StartTime = -1;
            NeedsFile = false;
        }

        private Pcb(int pid, Pcb parent, int priority, int remaningTime, string filePath)
        {
            this.Pid = pid;
            this.Name = $"Process_{pid}";
            Parent = parent ?? throw new InvalidParentException($"Parent cannot be null for process {pid}.");
            this.Priority = priority;
            this.RemaningTime = remaningTime;
            State = State.New;
            Children = new Dictionary<int, Pcb>();
            StartTime = -1;
            this.FilePath = filePath;
            NeedsFile = true;
        }
        

        public static Pcb Create(int pid, Pcb parent, int priority, int remaningTime, string filtePath)  
        {
            return new Pcb(pid, parent, priority, remaningTime, filtePath);
        }
        public static Pcb Create(int pid, Pcb parent, int priority, int remaningTime)
        {
            return new Pcb(pid, parent, priority, remaningTime);
        }

    }
}
