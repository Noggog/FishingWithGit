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
        static bool shouldLog;

        static void Main(string[] args)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.Arguments = string.Join(" ", args);
                WriteLine(DateTime.Now.ToString());
                WriteLine("Arguments:");
                WriteLine($"  {startInfo.Arguments}");
                WriteLine("");
                WriteLine("Arguments going in:");
                startInfo.Arguments = ProcessCommand(startInfo.Arguments);
                WriteLine($"  {startInfo.Arguments}");
                WriteLine("");
                WriteLine("Working directory " + Directory.GetCurrentDirectory());
                WriteLine("Running exe from " + System.Reflection.Assembly.GetEntryAssembly().Location);
                var trimIndex = System.Reflection.Assembly.GetEntryAssembly().Location.IndexOf(SourcePath);
                var trim = System.Reflection.Assembly.GetEntryAssembly().Location.Substring(trimIndex + SourcePath.Length);
                trim = TargetPath + trim;
                WriteLine("Target exe " + trim);
                startInfo.FileName = trim;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                startInfo.RedirectStandardOutput = true;
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
                shouldLog = true;
                WriteLine("An error occurred!!!: " + ex.Message);
            }

            try
            {
                if (!shouldLog) return;
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

        static string ProcessCommand(string str)
        {
            return EnsureFormatIsQuoted(str);
        }

        static string EnsureFormatIsQuoted(string str)
        {
            var toReplace = " --format=";
            int index = 0;
            while ((index = str.IndexOf(toReplace, index)) != -1)
            {
                index += toReplace.Length;
                bool insertQuote = str.Length > index + 1 && str[index + 1] != '\"';
                if (!insertQuote)
                {
                    continue;
                }
                shouldLog = true;
                WriteLine("Need to insert quotes for format at index " + index);
                str = str.Insert(index, "\"");
                var endIndex = str.IndexOf(" -", index);
                if (endIndex == -1)
                { // no parameter after format, just append
                    str += "\"";
                }
                else
                {
                    str = str.Insert(endIndex, "\"");
                }
            }
            return str;
        }

        static void WriteLine(string line)
        {
            sb.AppendLine(line);
        }
    }
}
