using System.Diagnostics.CodeAnalysis;
using DS_OS.DataBaseManager;
using DS_OS.DataStructer;
using DS_OS.Engine.CommandHandler.BaseClass;
using DS_OS.Engine.ProccesManager;
using DS_OS.Exceptions;
using DS_OS.FileManager;

namespace DS_OS.Engine.CommandHandler;

public partial class CommandHandler(IDataBaseManager dB,IProcessManager pm, IFileManager fM) : ICommandHandler
{
    private readonly IDataBaseManager _dB = dB;
    private readonly IProcessManager _pM = pm;
    private readonly IFileManager _fM = fM;
    public bool Handle(Command command)
    {
        throw new NotImplementedException();
    }

    public bool CreateProcessHandler(Command command)
    {
        Pcb parent = _dB.GetPcb(int.Parse(command.Parameters["parent"]));
        if (command.Parameters.Keys.Count == 5)
        {
            Pcb process = Pcb.Create(int.Parse(command.Parameters["pid"]),parent, int.Parse(command.Parameters["priority"]),
                int.Parse(command.Parameters["burst"]),
                command.Parameters["filepath"]);
        }
        else if (command.Parameters.Keys.Count == 4)
        {
            Pcb process = Pcb.Create(int.Parse(command.Parameters["pid"]), parent, int.Parse(command.Parameters["priority"]), int.Parse(command.Parameters["burst"]));
        }
        else
        {
            throw new InvalidProcessFormat();
        }
        _pM.Update();
        return true;
    }

    public bool CreateFileHandler(Command command)
    {
        _fM.AddFile(new FileNode(command.Parameters["name"], true));

    }

}   

