using DS_OS.Engine.CommandHandler.BaseClass;

namespace DS_OS.Engine.CommandHandler;

public interface ICommandHandler
{
    bool Handle(Command command);

}