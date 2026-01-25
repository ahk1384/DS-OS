using DS_OS.Engine.CommandHandler.BaseClass;

namespace DS_OS.Parser;

public interface IQueryParser
{
    Command Parse(String query);
}