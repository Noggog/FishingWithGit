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
        static StringBuilder sb = new StringBuilder();

        static void Main(string[] args)
        {
            try
            {
                WriteLine("Arguments:");
                WriteLine("  " + string.Join(" ", args));
                WriteLine("");
                WriteLine("Working directory " + Directory.GetCurrentDirectory());
                WriteLine("Running exe from " + System.Reflection.Assembly.GetEntryAssembly().Location);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                var trimIndex = System.Reflection.Assembly.GetEntryAssembly().Location.IndexOf(SourcePath);
                var trim = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(trimIndex + SourcePath.Length);
                trim = TargetPath + trim;
                WriteLine("Target exe " + trim);
                startInfo.FileName = trim;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                startInfo.RedirectStandardOutput = true;
                startInfo.Arguments = string.Join(" ", args);
                startInfo.UseShellExecute = false;
                using (var process = Process.Start(startInfo))
                {
                    bool first = true;
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            if (first)
                            {
                                WriteLine("--------- Standard Output :");
                                first = false;
                            }
                            Console.Write(result);
                            WriteLine(result);
                        }
                    }
                    first = true;
                    using (StreamReader reader = process.StandardError)
                    {
                        string result = reader.ReadToEnd();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            if (first)
                            {
                                WriteLine("--------- Standard Error :");
                                first = false;
                            }
                            Console.Error.Write(result);
                            WriteLine(result);
                        }
                    }
                    process.WaitForExit();
                    WriteLine("Exit Code: " + process.ExitCode);
                }
                WriteLine("--------------------------------------------------------------------------------------------------------- Fishing With Git call done.");

            }
            catch (Exception ex)
            {
                WriteLine("An error occurred!!!: " + ex.Message);
            }

            try
            {
                DirectoryInfo curDir = new DirectoryInfo(Directory.GetCurrentDirectory());
                using (StreamWriter sw = File.AppendText(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + $"/Temp/FishingWithGit-{curDir.Name}.log"))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        static void WriteLine(string line)
        {
            sb.AppendLine(line);
        }
    }
}
