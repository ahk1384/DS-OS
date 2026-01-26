using DS_OS.Engine.CommandHandler.BaseClass;

namespace DS_OS.Engine.CommandHandler;

public interface ICommandHandler
{
    Task<bool> Handle(Command command);
}