using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_OS.DataStructer
{
    internal class PCB
    {
        public int pid { get; private set; }
        public string name { get; private set; }
        public int  priority { get; set; }
        public int  remaingTime { get; set; }
        public State state { get; set; }
        public bool needsFile { get; set; }
        public string FiltePath { get; set; } = null;
        public WaitReason WaitReason { get; set; }      
        public PCB Parent { get; set; }
        public List<PCB> Children { get; set; }
        public int StartTime { get; set; } = -1;
        public int FinishTime { get; set; }

    }
}
