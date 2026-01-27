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
            { CommandType.DIRECTORYCREATE, CreateDirectoryHandler},
            { CommandType.SHUTDOWN, ShutdownHandler }
        };
    }

    public async Task<bool> Handle(Command command)
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
            Console.WriteLine($"Process {process.Pid} created successfully");
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
            // Validate required parameters
            if (!command.Parameters.ContainsKey("path") || !command.Parameters.ContainsKey("name"))
            {
                Console.WriteLine("Error: Missing required parameters. Usage: FILECREATE(path:/your/path,name:filename)");
                return false;
            }

            string path = command.Parameters["path"];
            string name = command.Parameters["name"];
            
            // Optional size parameter
            long size = 0;
            if (command.Parameters.ContainsKey("size") && long.TryParse(command.Parameters["size"], out long parsedSize))
            {
                size = parsedSize;
            }

            // Validate path exists
            if (!_fM.Exists(path))
            {
                Console.WriteLine($"Error: Directory '{path}' does not exist");
                return false;
            }

            // Validate it's a directory
            if (!_fM.IsDirectory(path))
            {
                Console.WriteLine($"Error: '{path}' is not a directory");
                return false;
            }

            // Validate filename
            if (!_fM.IsValidFileName(name))
            {
                Console.WriteLine($"Error: '{name}' is not a valid filename");
                return false;
            }

            bool res = _fM.CreateFile(path, name, size);
            if (res)
            {
                Console.WriteLine($"File '{name}' created successfully in '{path}'");
                _pM.Update();
            }
            else
            {
                Console.WriteLine($"Failed to create file '{name}' in '{path}'. File may already exist.");
            }
            
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
            // Validate required parameters
            if (!command.Parameters.ContainsKey("path") || !command.Parameters.ContainsKey("name"))
            {
                Console.WriteLine("Error: Missing required parameters. Usage: DIRECTORYCREATE(path:/parent/path,name:directoryname)");
                return false;
            }

            string path = command.Parameters["path"];
            string name = command.Parameters["name"];

            // Validate path format
            if (!_fM.IsValidPath(path))
            {
                Console.WriteLine($"Error: '{path}' is not a valid path");
                return false;
            }

            // Validate directory name
            if (!_fM.IsValidFileName(name))
            {
                Console.WriteLine($"Error: '{name}' is not a valid directory name");
                return false;
            }

            // Check if parent path exists
            if (!_fM.Exists(path))
            {
                Console.WriteLine($"Error: Parent directory '{path}' does not exist");
                return false;
            }

            // Validate parent is a directory
            if (!_fM.IsDirectory(path))
            {
                Console.WriteLine($"Error: '{path}' is not a directory");
                return false;
            }

            // Check if directory already exists
            string fullPath = path.TrimEnd('/') + "/" + name;
            if (_fM.Exists(fullPath))
            {
                Console.WriteLine($"Error: Directory '{name}' already exists in '{path}'");
                return false;
            }

            // Create the directory
            bool result = _fM.CreateDirectory(path, name);
            
            if (result)
            {
                Console.WriteLine($"Directory '{name}' created successfully in '{path}'");
                Console.WriteLine($"Full path: {fullPath}");
                
                // Update process manager to check for any waiting file processes
                _pM.Update();
            }
            else
            {
                Console.WriteLine($"Failed to create directory '{name}' in '{path}'");
            }

            return result;
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
            // Validate required parameters
            if (!command.Parameters.ContainsKey("path"))
            {
                Console.WriteLine("Error: Missing required parameter. Usage: FILEDELETE(path:/full/path/to/file)");
                return false;
            }

            string path = command.Parameters["path"];

            // Check if file/directory exists
            if (!_fM.Exists(path))
            {
                Console.WriteLine($"Error: '{path}' does not exist");
                return false;
            }

            // Prevent deletion of root
            if (path == "/")
            {
                Console.WriteLine("Error: Cannot delete root directory");
                return false;
            }

            bool res = _fM.Delete(path);
            if (res)
            {
                Console.WriteLine($"'{path}' deleted successfully");
                _pM.Update();
            }
            else
            {
                Console.WriteLine($"Failed to delete '{path}'");
            }
            
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
            // Validate required parameters
            if (!command.Parameters.ContainsKey("pid"))
            {
                Console.WriteLine("Error: Missing required parameter. Usage: PROCESSDELETE(pid:123)");
                return false;
            }

            if (!int.TryParse(command.Parameters["pid"], out int pid))
            {
                Console.WriteLine("Error: PID must be a valid integer");
                return false;
            }

            // Check if process exists
            if (!_dB.ProcessExists(pid))
            {
                Console.WriteLine($"Error: Process with PID {pid} does not exist");
                return false;
            }

            // Prevent deletion of init process
            if (pid == 0)
            {
                Console.WriteLine("Error: Cannot delete init process (PID 0)");
                return false;
            }

            Pcb process = _dB.GetPcb(pid);
            bool res = _dB.DeleteProcess(process);
            
            if (res)
            {
                Console.WriteLine($"Process {pid} deleted successfully");
                _pM.Update();
            }
            else
            {
                Console.WriteLine($"Failed to delete process {pid}");
            }
            
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
            Console.WriteLine("System shutting down...");
            
            // Graceful shutdown sequence
            Console.WriteLine("Stopping process executor...");
            // The executor should be stopped by the main program
            
            Console.WriteLine("Cleaning up resources...");
            _dB.Clear();
            
            Console.WriteLine("Shutdown complete.");
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

    #region Helper Methods

    /// <summary>
    /// Display help information for all supported commands
    /// </summary>
    public void DisplayHelp()
    {
        Console.WriteLine("\n=== DS-OS Command Help ===");
        Console.WriteLine("Supported Commands:");
        Console.WriteLine();
        
        Console.WriteLine("PROCESS COMMANDS:");
        Console.WriteLine("  PROCESSCREATE(pid:123,parent:0,priority:5,burst:100)");
        Console.WriteLine("  PROCESSCREATE(pid:124,parent:0,priority:3,burst:50,filepath:/data/input.txt)");
        Console.WriteLine("  PROCESSDELETE(pid:123)");
        Console.WriteLine();
        
        Console.WriteLine("FILE SYSTEM COMMANDS:");
        Console.WriteLine("  DIRECTORYCREATE(path:/,name:home)");
        Console.WriteLine("  DIRECTORYCREATE(path:/home,name:user1)");
        Console.WriteLine("  FILECREATE(path:/home,name:test.txt)");
        Console.WriteLine("  FILECREATE(path:/home,name:data.bin,size:1024)");
        Console.WriteLine("  FILEDELETE(path:/home/test.txt)");
        Console.WriteLine();
        
        Console.WriteLine("SYSTEM COMMANDS:");
        Console.WriteLine("  SHUTDOWN()");
        Console.WriteLine();
        
        Console.WriteLine("Parameter Types:");
        Console.WriteLine("  pid     - Process ID (integer)");
        Console.WriteLine("  parent  - Parent process ID (integer)");
        Console.WriteLine("  priority- Process priority (integer)");
        Console.WriteLine("  burst   - Process burst time (integer)");
        Console.WriteLine("  path    - Directory path (string)");
        Console.WriteLine("  name    - File/directory name (string)");
        Console.WriteLine("  size    - File size in bytes (integer, optional)");
        Console.WriteLine("  filepath- File path for process (string, optional)");
        Console.WriteLine();
    }

    #endregion
}

