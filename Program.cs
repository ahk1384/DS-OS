using DS_OS.Engine.CommandHandler.BaseClass;
using DS_OS.Parser;

namespace DS_OS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            QueryParser parser = new QueryParser();
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
