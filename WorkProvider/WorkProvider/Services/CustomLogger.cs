using System;

namespace WorkProvider.Services
{
    public static class CustomLogger
    {
        public static void Log(String[] args)
        {
            Console.WriteLine($"{DateTime.UtcNow.ToString("yyyy-mm-dd hh:mm:ss")}|{String.Join('|', args)}");
        }

        public static void LogInGray(String[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Log(args);
            Console.ResetColor();
        }
    }
}
