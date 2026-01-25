namespace DS_OS.Engine.CommandHandler.BaseClass;

public class Command(CommandType type, Dictionary<string, string> parameters)
{
    public CommandType Type { get; private set; } = type;

    public Dictionary<string, string> Parameters { get; private set; } = parameters;

    public void AddParams(string key, string value)
    {
        Parameters.Add(key, value);
    }
}
