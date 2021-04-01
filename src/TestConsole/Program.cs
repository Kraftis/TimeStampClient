using System;
using System.IO;

namespace TestConsole
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            string path = @"c:\temp\test.txt";

            Stream stream = File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader reader = new StreamReader(stream);
            string line = reader.ReadLine();

            var timeStampTokenUpdated = await Disig.TimeStampClient.TimeStampClient.RequestTimeStampTokenUpdatedAsync("https://localhost:44340/tsr", stream);

            Console.WriteLine("Test");
        }
    }
}
