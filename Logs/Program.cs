using System.IO.Pipes;
using System.Text;
namespace Logs
{
    public class Program
    {
        private static string message = "";
        static void Main(string[] args)
        {
            print("");
        }
        public static void print(string messages)
        {
            Console.WriteLine("🟢 Logger Console - Waiting for messages...");
            
            while (true)
            {
                using var server = new NamedPipeServerStream("MyLogPipe");

                server.WaitForConnection();

                using var reader = new StreamReader(server);
                string message = reader.ReadToEnd();
                if (message == "shutdown")
                {
                    break;
                }
                Console.WriteLine("--CYCLE SUMMARY-- \nPID | RESULT ");
                Console.WriteLine($"{message}");
            }
        }
    }

}
