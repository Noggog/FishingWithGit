using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class Program
    {
        const string TargetPath = "C:\\Program Files\\Git2\\";
        const string SourcePath = "C:\\Program Files\\Git\\";

        static void Main(string[] args)
        {
            try
            {
                //System.Console.WriteLine("Fishing With Git, " + string.Join(" ", args));
                //System.Console.WriteLine("Working directory " + Directory.GetCurrentDirectory());
                //System.Console.WriteLine("Running exe from " + System.Reflection.Assembly.GetEntryAssembly().Location);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                var trimIndex = System.Reflection.Assembly.GetEntryAssembly().Location.IndexOf(SourcePath);
                var trim = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(trimIndex + SourcePath.Length);
                trim = TargetPath + trim;
                //System.Console.WriteLine("Target exe " + trim);
                startInfo.FileName = trim;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = string.Join(" ", args);
                startInfo.UseShellExecute = false;
                using (var process = Process.Start(startInfo))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                    using (StreamReader reader = process.StandardError)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                    process.WaitForExit();
                    //System.Console.WriteLine(process.ExitCode);
                }
                if (startInfo.Arguments.Contains("checkout"))
                {
                    System.Console.WriteLine("Fishing!");
                }
                //System.Console.WriteLine("Fishing With Git Done. ");

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred!!!: " + ex.Message);
                return;
            }

        }
    }
}
