using DS_OS.DataBaseManager;
using DS_OS.DataStructer;
using DS_OS.Engine.CommandHandler.BaseClass;
using DS_OS.Engine.ProccesManager;
using DS_OS.Exceptions;
using DS_OS.FileManager;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace DS_OS.Engine.CommandHandler;

public partial class CommandHandler: ICommandHandler
{
    private readonly IDataBaseManager _dB ;
    private readonly IProcessManager _pM;
    private readonly IFileManager _fM;

    private readonly Dictionary<CommandType, Func<Command, bool>> _handlers;

    public CommandHandler(IDataBaseManager dB, IProcessManager pm, IFileManager fM)
    {
        _dB = dB;
        _pM = pm;
        _fM = fM;
        
        _handlers = new Dictionary<CommandType, Func<Command, bool>>
        {
            { CommandType.PROCESSCREATE, CreateProcessHandler },
            { CommandType.FILECREATE, CreateFileHandler },
            { CommandType.FILEDELETE, DeleteFileHandler },
            { CommandType.PROCESSDELETE, DeleteProcessHandler },
            { CommandType.DIRECTORYCREATE ,CreateDirectoryHandler},
            { CommandType.SHUTDOWN, ShutdownHandler }
        };
    }

    public bool Handle(Command command)
    {
        try
        {
            if (_handlers.TryGetValue(command.Type, out var handler))
            {
                return handler(command);
            }
            
            Console.WriteLine($"Unknown command type: {command.Type}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling command: {ex.Message}");
            return false;
        }
    }

    private bool CreateProcessHandler(Command command)
    {
        try
        {
            Pcb? process;
            Pcb parent = _dB.GetPcb(int.Parse(command.Parameters["parent"]));
            
            if (command.Parameters.Keys.Count == 5)
            {
                process = Pcb.Create(
                    int.Parse(command.Parameters["pid"]),
                    parent, 
                    int.Parse(command.Parameters["priority"]),
                    int.Parse(command.Parameters["burst"]),
                    command.Parameters["filepath"]);
            }
            else if (command.Parameters.Keys.Count == 4)
            {
                process = Pcb.Create(
                    int.Parse(command.Parameters["pid"]), 
                    parent, 
                    int.Parse(command.Parameters["priority"]), 
                    int.Parse(command.Parameters["burst"]));
            }
            else
            {
                throw new InvalidProcessFormat();
            }
            
            _dB.AddProcess(process);
            _pM.Update();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating process: {ex.Message}");
            return false;
        }
    }

    private bool CreateFileHandler(Command command)
    {
        try
        {
            bool res = _fM.CreateFile(command.Parameters["path"], command.Parameters["name"]);
            _pM.Update();
            return res;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating file: {ex.Message}");
            return false;
        }
    }

    private bool CreateDirectoryHandler(Command command)
    {
        try
        {
            bool res = _fM.CreateDirectory(command.Parameters["path"], command.Parameters["name"]); 
            _pM.Update();
            return res;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating directory: {ex.Message}");
            return false;
        }
    }

    private bool DeleteFileHandler(Command command)
    {
        try
        {
            bool res = _fM.Delete(command.Parameters["path"]);
            _pM.Update();
            return res;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting file: {ex.Message}");
            return false;
        }
    }

    private bool DeleteProcessHandler(Command command)
    {
        try
        {
            int pid = int.Parse(command.Parameters["pid"]);
            Pcb process = _dB.GetPcb(pid);
            bool res = _dB.DeleteProcess(process);
            _pM.Update();
            return res;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting process: {ex.Message}");
            return false;
        }
    }

    private bool ShutdownHandler(Command command)
    {
        try
        {
            // Implement shutdown logic here
            Console.WriteLine("System shutting down...");
            Environment.Exit(0);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during shutdown: {ex.Message}");
            return false;
        }
    }

    public void RegisterHandler(CommandType commandType, Func<Command, bool> handler)
    {
        _handlers[commandType] = handler;
    }

    public bool HasHandler(CommandType commandType)
    {
        return _handlers.ContainsKey(commandType);
    }

    public IEnumerable<CommandType> GetSupportedCommands()
    {
        return _handlers.Keys;
    }
}

