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
        HookManager hookMgr;

        public BaseWrapper()
        {
            hookMgr = new HookManager(this);
        }

        public int Wrap(string[] args)
        {
            try
            {
                var startInfo = GetStartInfo(args);
                HookPair hook = null;
                if (Properties.Settings.Default.FireHookLogic)
                {
                    hook = hookMgr.GetHook(args);
                    if (hook == null)
                    {
                        this.WriteLine("No hooks for this command.");
                    }
                    else if (hook.PreCommand != null)
                    {
                        WriteLine("Firing prehooks.");
                        int? hookExitCode = hook?.PreCommand?.Invoke();
                        WriteLine("Fired prehooks.");
                        if (0 != (hookExitCode ?? 0))
                        {
                            WriteLine("Exiting early because of hook failure.");
                            return hookExitCode.Value;
                        }
                    }
                }
                else
                {
                    WriteLine("Fire hook logic is off.");
                }
                int exitCode;
                if (args.Contains("-NO_PASSING_FISH"))
                {
                    exitCode = 0;
                }
                else
                {
                    exitCode = RunProcess(startInfo);
                }
                if (Properties.Settings.Default.FireHookLogic
                    && hook?.PostCommand != null)
                {
                    WriteLine("Firing posthooks.");
                    int? hookExitCode = hook?.PostCommand?.Invoke();
                    WriteLine("Fired posthooks.");
                    if (0 != (hookExitCode ?? 0))
                    {
                        WriteLine("Exiting early because of hook failure.");
                        return hookExitCode.Value;
                    }
                }
                return exitCode;
            }
            catch (Exception ex)
            {
                shouldLog = true;
                WriteLine("An error occurred!!!: " + ex.Message, writeToConsole: true);
                throw;
            }
            finally
            {
                WriteLine("--------------------------------------------------------------------------------------------------------- Fishing With Git call done.");
                if (this.shouldLog)
                {
                    LogResults();
                }
            }
        }

        ProcessStartInfo GetStartInfo(string[] args)
        {
            WriteLine(DateTime.Now.ToString());
            ProcessStartInfo startInfo = new ProcessStartInfo();
            WriteLine("Arguments:");
            WriteLine($"  {string.Join(" ", args)}");
            WriteLine("");
            args = ProcessCommand(args);
            startInfo.Arguments = string.Join(" ", args);
            WriteLine("Arguments going in:");
            WriteLine($"  {startInfo.Arguments}");
            WriteLine("");
            WriteLine("Working directory " + Directory.GetCurrentDirectory());
            var exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            if (exePath.EndsWith("FishingWithGit.exe"))
            {
                exePath = exePath.Replace("FishingWithGit.exe", "git.exe");
            }
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

        public int RunProcess(ProcessStartInfo startInfo)
        {
            FileInfo file = new FileInfo(startInfo.FileName);
            if (!file.Exists)
            {
                throw new ArgumentException("File does not exist: " + file.FullName);
            }
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
                return process.ExitCode;
            }
        }

        string[] ProcessCommand(string[] args)
        {
            args = StripCArguments(args);
            args = StripTemplateArguments(args);
            return EnsureFormatIsQuoted(args);
        }

        string[] StripCArguments(string[] args)
        {
            if (!Properties.Settings.Default.CleanCArguments) return args;
            List<string> argsList = args.ToList();
            int index;
            while ((index = argsList.IndexOf("-c")) != -1)
            {
                argsList.RemoveAt(index);
                argsList.RemoveAt(index);
            }
            return argsList.ToArray();
        }

        string[] StripTemplateArguments(string[] args)
        {
            if (!Properties.Settings.Default.RemoveTemplateFromClone) return args;
            if (!args.Contains("clone")) return args;
            return args
                .Where((arg) => !arg.StartsWith("--template"))
                .ToArray();
        }

        string[] EnsureFormatIsQuoted(string[] args)
        {
            var formatStr = "--format=";
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith(formatStr))
                {
                    if (arg.Length <= formatStr.Length) continue;
                    if (arg[formatStr.Length + 1] == '\"') continue;
                    if (arg[arg.Length - 1] == '\"') continue;
                    WriteLine("Need to insert quotes for format.");
                    args[i] = $"\"{arg}\"";
                }
            }
            return args;
        }

        public void WriteLine(string line, bool writeToConsole = false)
        {
            if (writeToConsole)
            {
                System.Console.WriteLine(line);
            }
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
