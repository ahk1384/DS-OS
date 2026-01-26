using DS_OS.Engine.CommandHandler.BaseClass;
using DS_OS.Engine.ProccesManager;
using DS_OS.Engine.ProcessExcuter;
using DS_OS.DataBaseManager;
using DS_OS.Engine.ProcessExecutor;
using DS_OS.FileManager;
using DS_OS.Parser;

namespace DS_OS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IFileManager fileManager = new FileManager.FileManager();
            IDataBaseManager dataBaseManager = new DataBaseManager.DataBaseManager(fileManager);
            IProcessManager processManager = new ProcessManager(dataBaseManager, fileManager);
            IProcessExecutor processExecutor = new ProcessExecutor(dataBaseManager, processManager, fileManager);
            ConfigureSettings settings = new ConfigureSettings(processManager, processExecutor, dataBaseManager);
            if (settings.LoadSettings())
            {
                QueryParser parser = new QueryParser();
                processExecutor.Start();
                while (true)
                {
                    try
                    {

                        Command command = parser.Parse(Console.ReadLine());
                        foreach (KeyValuePair<string, string> commandParameter in command.Parameters)
                        {
                            Console.WriteLine($"the key is {commandParameter.Key} and the value is {commandParameter.Value}");
                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e.Message);
                    }
                }
            }
           

            
        }
    }
}
