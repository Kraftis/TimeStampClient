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

            Stream stream = File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var timeStampTokenUpdated = await Disig.TimeStampClient.TimeStampClient.RequestTimeStampTokenUpdatedAsync("https://localhost:5001/tsr", ms);
            stream.Close();
            //var timeStampTokenUpdatedd = Disig.TimeStampClient.TimeStampClient.RequestTimeStampToken("https://localhost:44340/tsr", b);

            SaveToAsicSimple(path, timeStampTokenUpdated);

            Console.WriteLine("Test Done");
        }

        public static void SaveToAsicSimple(string inputFile, TimeStampToken timeStampToken)
        {
            using (Ionic.Zip.ZipFile zipFile = new Ionic.Zip.ZipFile(UTF8Encoding.UTF8))
            {
                zipFile.ParallelDeflateThreshold = -1;
                zipFile.UseZip64WhenSaving = Ionic.Zip.Zip64Option.Never;
                zipFile.EmitTimesInUnixFormatWhenSaving = false;
                zipFile.EmitTimesInWindowsFormatWhenSaving = false;
                zipFile.Comment = @"mimetype=application/vnd.etsi.asic-s+zip";

                using (System.IO.FileStream inputStream = new System.IO.FileStream(inputFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (MemoryStream ms = new MemoryStream())
                {
                    zipFile.AddEntry(@"mimetype", System.Text.UTF8Encoding.UTF8.GetBytes(@"application/vnd.etsi.asic-s+zip"));
                    zipFile.AddEntry(System.IO.Path.GetFileName(inputFile), inputStream);
                    zipFile.AddEntry(@"META-INF/timestamp.tst", timeStampToken.ToByteArray());
                    zipFile.Save(ms);
                }

                Console.WriteLine("Zip created");
            }
        }
    }
}
