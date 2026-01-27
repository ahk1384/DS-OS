using DS_OS.Engine.CommandHandler.BaseClass;
using DS_OS.Engine.ProccesManager;
using DS_OS.Engine.ProcessExcuter;
using DS_OS.DataBaseManager;
using DS_OS.Engine.ProcessExecutor;
using DS_OS.FileManager;
using DS_OS.Parser;
using System;
using System.Threading;
using System.Threading.Tasks;
using DS_OS.Engine.CommandHandler;

namespace DS_OS
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine("=== DS-OS Starting ===");
            
            // Create core components
            IFileManager fileManager = new FileManager.FileManager();
            IDataBaseManager dataBaseManager = new DataBaseManager.DataBaseManager(fileManager);
            IProcessManager processManager = new ProcessManager(dataBaseManager, fileManager);
            IProcessExecutor processExecutor = new ProcessExecutor(dataBaseManager, processManager, fileManager);
            ConfigureSettings settings = new ConfigureSettings(processManager, processExecutor, dataBaseManager);
            ICommandHandler handler = new CommandHandler(dataBaseManager, processManager, fileManager);

            // Cancellation support for graceful shutdown
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                // prevent the process from terminating immediately
                e.Cancel = true;
                Console.WriteLine("\nShutdown requested, stopping executor...");
                try
                {
                    processExecutor.End();
                }
                catch { /* swallow exceptions during shutdown */ }

                // signal cancellation to main loop
                cts.Cancel();
            };

            try
            {
                Console.WriteLine("Loading system settings...");
                if (!settings.LoadSettings())
                {
                    Console.WriteLine("Failed to load settings. Exiting."); 
                    return 1;
                }

                Console.WriteLine("Starting process executor...");
                // Start the process executor on a background task so it doesn't block the REPL
                Task executorTask = Task.Run(() => processExecutor.Start(), cts.Token);

                // Basic REPL for parsing commands
                var parser = new QueryParser();
                
                // Display welcome message and help
                Console.WriteLine("=== DS-OS Ready ===");
                Console.WriteLine("Operating System Simulator is now running.");
                Console.WriteLine("Type commands to interact with the system.");
                Console.WriteLine("Examples:");
                Console.WriteLine("  DIRECTORYCREATE(path:/,name:home)");
                Console.WriteLine("  FILECREATE(path:/,name:readme.txt)");
                Console.WriteLine("  PROCESSCREATE(pid:1,parent:0,priority:5,burst:100)");
                Console.WriteLine("  HELP() - for complete command list");
                Console.WriteLine("  SHUTDOWN() - to exit");
                Console.WriteLine("  Ctrl+C - emergency exit");
                Console.WriteLine(new string('=', 50));

                while (!cts.IsCancellationRequested)
                {
                    Console.Write("DS-OS> ");
                    string? line;
                    try
                    {
                        line = Console.ReadLine();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    if (line == null)
                    {
                        // EOF reached (e.g., redirected input ended)
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Handle special commands
                    if (line.Trim().Equals("help", StringComparison.OrdinalIgnoreCase) || 
                        line.Trim().Equals("?", StringComparison.OrdinalIgnoreCase))
                    {
                        ((CommandHandler)handler).DisplayHelp();
                        continue;
                    }

                    if (line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) || 
                        line.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine("Exiting DS-OS...");
                        break;
                    }

                    try
                    {
                        Command command = parser.Parse(line);
                        
                        // Show parsed command for debugging (can be removed later)
                        Console.WriteLine($"Command: {command.Type}");
                        handler.Handle(command);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Type 'help' for command syntax help.");
                    }
                }

                // Shutdown sequence
                Console.WriteLine("Initiating shutdown...");
                cts.Cancel();
                try
                {
                    processExecutor.End();
                }
                catch { }

                // Wait a short time for executor to stop
                await Task.WhenAny(executorTask, Task.Delay(2000));
                Console.WriteLine("DS-OS shutdown complete.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                try
                {
                    processExecutor.End();
                }
                catch { }
                return 2;
            }
        }
    }
}
