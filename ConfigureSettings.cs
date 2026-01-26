using System.Runtime.InteropServices;
using System.Text.Json;
using DS_OS.DataBaseManager;
using DS_OS.Engine.ProccesManager;
using DS_OS.Engine.ProcessExcuter;

namespace DS_OS;

public class ConfigureSettings(
    IProcessManager processManager,
    IProcessExecutor processExecutor,
    IDataBaseManager dataBaseManager)
{
    private readonly IProcessManager _processManager = processManager;
    private readonly IProcessExecutor _processExecutor = processExecutor;
    private readonly IDataBaseManager _dataBaseManager = dataBaseManager;

    public bool LoadSettings()
    {
        Dictionary<string,int> config = ReadFile();  
        _processExecutor.Config(
            config.GetValueOrDefault("ExecutionPerCycle"),
            config.GetValueOrDefault("QuantumSize")
        );
        _processManager.Config(config.GetValueOrDefault("Prf"));
        _dataBaseManager.Config(config.GetValueOrDefault("ReadyLimit"));
        return true;
    }

    private static Dictionary<string, int>? ReadFile()
    {
        try
        {
            string jsonString = File.ReadAllText("E:\\Amir\\git hub repository work\\DS-OS\\DS-OS\\Settings.json");
            var dict = JsonSerializer.Deserialize<Dictionary<string, int>>(jsonString);
            return dict;
        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
        }
        return null;
        
    }
}