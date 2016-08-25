using System;
namespace Win32Bridge
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Start Win32Bridge");

            var app = new AppService();
            app.RunAppService();

            var readData = "";
            while (readData != "quit")
            {
                readData = Console.ReadLine();
            }
        }
    }
}
