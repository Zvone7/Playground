using WorkProvider.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace WorkProvider
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // A: 
            // C# class to execute a number of pieces of work (actions) 
            // in the background (i.e. without blocking the program’s execution)

            // B:
            // Application that sequentially writes files to the file system in queue like manner.


            String selector = "";
            while (!selector.Equals("0"))
            {
                Console.WriteLine("\nHello! Select which task to run. (A or B, 0 to exit)");
                selector = Console.ReadLine();
                if (selector.ToLower(CultureInfo.InvariantCulture).Equals("a"))
                {
                    var taskRunner = new TaskRunner(new WorkDequeuerTaskA(), 30);
                    await taskRunner.RunTaskAsync();
                }
                else if (selector.ToLower(CultureInfo.InvariantCulture).Equals("b"))
                {
                    var taskRunner = new TaskRunner(new WorkDequeuerTaskB(), 30);
                    await taskRunner.RunTaskAsync();
                }
            }

            CustomLogger.Log(new String[] { "Have a nice day!" });
        }

    }

}
