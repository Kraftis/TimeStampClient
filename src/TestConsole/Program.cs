using Disig.TimeStampClient;
using System;
using System.IO;
using System.Text;

namespace TestConsole
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string path = @"c:\temp\test.txt";

            //TimeStampToken timeStampTokenUpdated;
            //using (Stream stream = File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            //{
            //    timeStampTokenUpdated = await Disig.TimeStampClient.TimeStampClient.RequestTimeStampTokenUpdatedAsync("https://localhost:5001/tsa", stream);
            //}

            //// SaveToAsicSimple(path, timeStampTokenUpdated, @"c:\temp\test.asics");
            //SaveToAsicSimple(path, timeStampTokenUpdated, @".\test.asics");

            Stream stream = File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;
            var timeStampTokenUpdated = await Disig.TimeStampClient.TimeStampClient.RequestTimeStampTokenUpdatedAsync("https://localhost:5001/tsa", ms);
            stream.Dispose();

            SaveToAsicSimple(path, timeStampTokenUpdated, @"c:\temp\test.asics");

            Console.WriteLine("Test Done");
        }

        public static void SaveToAsicSimple(string inputFile, TimeStampToken timeStampToken, string outputFile)
        {
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(UTF8Encoding.UTF8))
            {
                zipFile.ParallelDeflateThreshold = -1;
                zipFile.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Never;
                zipFile.EmitTimesInUnixFormatWhenSaving = false;
                zipFile.EmitTimesInWindowsFormatWhenSaving = false;
                zipFile.Comment = @"mimetype=application/vnd.etsi.asic-s+zip";

                using (System.IO.FileStream inputStream = new System.IO.FileStream(inputFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (System.IO.FileStream outputStream = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    zipFile.AddEntry(@"mimetype", System.Text.UTF8Encoding.UTF8.GetBytes(@"application/vnd.etsi.asic-s+zip"));
                    zipFile.AddEntry(System.IO.Path.GetFileName(inputFile), inputStream);
                    zipFile.AddEntry(@"META-INF/timestamp.tst", timeStampToken.ToByteArray());
                    zipFile.Save(outputStream);
                }

                Console.WriteLine("Zip created");
            }
        }
    }
}
