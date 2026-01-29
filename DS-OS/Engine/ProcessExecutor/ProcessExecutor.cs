using DS_OS.DataBaseManager;
using DS_OS.DataStructer;
using DS_OS.Engine.ProccesManager;
using DS_OS.Engine.ProcessExcuter;
using DS_OS.FileManager;
using System.Threading.Tasks;
using DS_OS.Logger;

namespace DS_OS.Engine.ProcessExecutor;

public class ProcessExecutor(IDataBaseManager dataBaseManager, IProcessManager processManager,IFileManager fileManager, ILogger logger,ILogger fileLogger) : IProcessExecutor
{
    private readonly ILogger _logger = logger;
    private readonly ILogger _fileLogger = fileLogger;
    private readonly IDataBaseManager _db = dataBaseManager;
    private readonly IProcessManager _pm = processManager;
    private readonly IFileManager _fm = fileManager;
    private bool _runState = true;
    private List<string> messages = new List<string>();
    private int QuantumSize { get; set; }
    private int ExecutionPerCycle { get; set; }

    public int Prf { private get; set; }

    public void Start()
    {
        _ = ExecuteProcess();
    }

    private async Task ExecuteProcess()
    {
        while (_runState)
        {
            messages.Clear();
            for (int i = 0; i < ExecutionPerCycle; i++)
            {
                Pcb? process = GetProcess();
                Execute(process);
            }
            _logger.Log(messages.ToArray());
            Thread.Sleep(QuantumSize * 100);
        }
    }

    private void Execute(Pcb? process)
    {
        try
        {
            if (process != null)
            {
                if (_pm.CheckFileExist(process).Result)
                {
                    process.State = State.Running;
                    process.RemaningTime -= 1;
                    process.RunedQuantum += 1;
                    if (process.RemaningTime > 0)
                    {
                        _pm.TimeOut(process);
                    }
                    else
                    {

                        _db.DeleteProcess(process);
                        _pm.CheckReadyLimit();
                    }
                    messages.Add(process.Pid + "  | " + nameof(LongType.EXECUTED));
                }
                else
                {
                    if (process.WaitReason == WaitReason.File)
                    {
                        messages.Add(process.Pid + "  | " + nameof(LongType.WATING_FILE));
                    }
                }
            }
            else
            {
                //the None output implement here
            }
        }
        catch (Exception e)
        {
            messages.Add(process.Pid + "  | " + nameof(LongType.ERROR));
        }

        
    }

    private Pcb? GetProcess()
    {
        if (!_db.IsReadyQueueEmpty())
        {
            return _db.GetNextReadyProcess();
        }
        else
        {
            return null;
        }
    }

    

    public bool End()
    {
        _runState = false;
        _logger.SaveLog();
        _logger.Log(LongType.SHUTDOWN.ToString());
        _fileLogger.Log(_fm.GetAllFilesRecursive("/"));
        return true;
    }

    public bool Config(int executionPerCycle,int quantumSize)
    {
        this.ExecutionPerCycle = executionPerCycle;
        this.QuantumSize = quantumSize;
        return true;
    }
}