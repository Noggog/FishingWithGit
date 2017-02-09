using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class BaseWrapper
    {
        StringBuilder sb = new StringBuilder();
        bool shouldLog = Properties.Settings.Default.ShouldLog;
        HookManager hookMgr = new HookManager();

        public void Wrap(string[] args)
        {
            try
            {
                var hook = hookMgr.GetHook(args);
                var startInfo = GetStartInfo(args);
                if (hook?.PreCommand != null)
                {
                    hook.PreCommand();
                }
                RunGitProcess(startInfo);
                if (hook?.PostCommand != null)
                {
                    hook.PostCommand();
                }
            }
            catch (Exception ex)
            {
                shouldLog = true;
                WriteLine("An error occurred!!!: " + ex.Message);
            }

            if (this.shouldLog)
            {
                LogResults();
            }
        }

        ProcessStartInfo GetStartInfo(string[] args)
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
            var exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            WriteLine("Running exe from " + exePath);
            var sourcePath = GetSourcePath();
            var trimIndex = exePath.IndexOf(sourcePath);
            var trim = exePath.Substring(trimIndex + sourcePath.Length);
            trim = Properties.Settings.Default.RealGitProgramFolder + trim;
            WriteLine("Target exe " + trim);
            startInfo.FileName = trim;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            return startInfo;
        }

        string GetSourcePath()
        {
            return Properties.Settings.Default.BackupSourcePath;
        }

        void RunGitProcess(ProcessStartInfo startInfo)
        {
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

        string ProcessCommand(string str)
        {
            return EnsureFormatIsQuoted(str);
        }

        string EnsureFormatIsQuoted(string str)
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

        void WriteLine(string line)
        {
            sb.AppendLine(line);
        }

        void LogResults()
        {
            try
            {
                DirectoryInfo curDir = new DirectoryInfo(Directory.GetCurrentDirectory());
                var filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + $"/Temp/FishingWithGit-{curDir.Name}.log";

                FileInfo file = new FileInfo(filePath);
                if (file.Exists
                    && (DateTime.Now - file.LastWriteTime).TotalDays > Properties.Settings.Default.WipeLogsOlderThanDays)
                {
                    file.Delete();
                }

                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(sb.ToString());
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
