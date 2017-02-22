using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class BaseWrapper
    {
        StringBuilder sb = new StringBuilder();
        bool shouldLog = Properties.Settings.Default.ShouldLog;
        CommandType commandType;
        int commandIndex;
        List<string> args;

        public BaseWrapper(string[] args)
        {
            this.args = args.ToList();
        }

        public int Wrap()
        {
            try
            {
                WriteLine(DateTime.Now.ToString());
                WriteLine("Arguments:");
                WriteLine($"  {string.Join(" ", args)}");
                WriteLine("");
                GetCommandInfo();
                WriteLine($"Command: {commandType.ToString()}", writeToConsole: !commandType.Silent());
                ProcessArgs();
                GetMainCommand(out commandIndex);
                var startInfo = GetStartInfo();
                HookSet hook = null;
                if (Properties.Settings.Default.FireHookLogic)
                {
                    hook = GetHook();
                    if (hook == null)
                    {
                        this.WriteLine("No hooks for this command.");
                    }
                    else
                    {
                        WriteLine("Firing prehooks.");
                        int? hookExitCode = hook.PreCommand();
                        WriteLine("Fired prehooks.");
                        if (0 != (hookExitCode ?? 0))
                        {
                            WriteLine($"Exiting early because of hook failure ({hookExitCode})", writeToConsole: true);
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
                    && hook != null)
                {
                    WriteLine("Firing posthooks.");
                    int? hookExitCode = hook.PostCommand();
                    WriteLine("Fired posthooks.");
                    if (0 != (hookExitCode ?? 0))
                    {
                        WriteLine($"Exiting early because of hook failure ({hookExitCode})", writeToConsole: true);
                        return hookExitCode.Value;
                    }
                }
                return exitCode;
            }
            catch (Exception ex)
            {
                shouldLog = true;
                WriteLine("An error occurred!!!: " + ex.Message, writeToConsole: true);
                shouldLog = true;
                WriteLine(ex.ToString(), writeToConsole: true);
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

        void GetCommandInfo(bool yell = false)
        {
            string cmdStr = GetMainCommand(out commandIndex);
            if (commandIndex == -1)
            {
                commandType = CommandType.unknown;
                if (yell)
                {
                    WriteLine("No command found.");
                    throw new ArgumentException("No command found.");
                }
                else
                {
                    return;
                }
            }
            if (!CommandTypeExt.TryParse(cmdStr, out commandType))
            {
                commandType = CommandType.unknown;
            }
        }

        ProcessStartInfo GetStartInfo()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Join(" ", args);
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
                        WriteLine(result, writeToConsole: true);
                    }
                }
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        private void ProcessArgs()
        {
            this.WriteLine("Processing generic arguments.");
            ArgProcessor.GenericProcessing(args);
            ArgProcessor commandProcessor;

            try
            {
                switch (commandType)
                {
                    case CommandType.clone:
                        commandProcessor = new CloneProcessor();
                        break;
                    case CommandType.add:
                        commandProcessor = new AddProcessor();
                        break;
                    case CommandType.reset:
                        commandProcessor = new ResetProcessor();
                        break;
                    default:
                        return;
                }
                this.WriteLine("Processing specialized arguments.");
                commandProcessor.Process(args);
            }
            finally
            {
                WriteLine("Arguments going in:");
                WriteLine($"  {string.Join(" ", args)}");
                WriteLine("");
            }
        }

        #region Logging
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
        #endregion

        #region Getting Hooks
        public HookSet GetHook()
        {
            switch (commandType)
            {
                case CommandType.checkout:
                    return CheckoutHooks.Factory(this, args, commandIndex);
                case CommandType.rebase:
                    return RebaseHooks.Factory(this, args, commandIndex);
                case CommandType.reset:
                    return ResetHooks.Factory(this, args, commandIndex);
                case CommandType.commitmsg:
                    return CommitMsgHooks.Factory(this, args, commandIndex);
                case CommandType.commit:
                    return CommitHooks.Factory(this, args, commandIndex);
                case CommandType.status:
                    return StatusHooks.Factory(this, args, commandIndex);
                default:
                    return null;
            }
        }

        private string GetMainCommand(out int index)
        {
            for (int i = 0; i < args.Count; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    continue;
                }
                if (args[i].StartsWith("-"))
                {
                    i++;
                    continue;
                }

                index = i;
                return args[i];
            }

            index = -1;
            return null;
        }
        #endregion

        #region Firing Hooks
        public int FireAllHooks(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
                () => FireBashHook(type, location, args),
                () => FireExeHooks(type, location, args));
        }

        public int FireUnnaturalHooks(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
                () => FireUntiedBashHook(type, location, args),
                () => FireExeHooks(type, location, args));
        }

        private string GetHookFolder(HookLocation location)
        {
            var path = Directory.GetCurrentDirectory();
            if (location == HookLocation.Normal)
            {
                path += "/.git";
            }
            path += "/hooks";
            return path;
        }

        private int FireBashHook(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
               () => FireNamedBashHook(type, location, args),
               () => FireUntiedBashHook(type, location, args));
        }

        private int FireNamedBashHook(HookType type, HookLocation location, params string[] args)
        {
            var path = GetHookFolder(location);
            FileInfo file = new FileInfo($"{path}/{type.HookName()}");
            if (!file.Exists) return 0;

            WriteLine($"Firing Named Bash Hook {location} {type.HookName()}", writeToConsole: !type.AssociatedCommand().Silent());

            var exitCode = RunProcess(
                SetArgumentsOnStartInfo(
                    new ProcessStartInfo(file.FullName, string.Join(" ", args))));

            WriteLine($"Fired Named Bash Hook {location} {type.HookName()}", writeToConsole: !type.AssociatedCommand().Silent());
            return exitCode;
        }

        private int FireUntiedBashHook(HookType type, HookLocation location, params string[] args)
        {
            var path = GetHookFolder(location);
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) return 0;

            foreach (var file in dir.EnumerateFiles())
            {
                if (!file.Extension.ToUpper().Equals(string.Empty)) continue;
                var rawName = Path.GetFileNameWithoutExtension(file.Name);
                if (HookTypeExt.IsHookName(rawName)) continue;

                WriteLine($"Firing Untied Bash Hook {location} {type.HookName()}: {file.Name}", writeToConsole: !type.AssociatedCommand().Silent());

                var exitCode = this.RunProcess(
                    SetArgumentsOnStartInfo(
                        new ProcessStartInfo(file.FullName, string.Join(" ", args))));

                WriteLine($"Fired Untied Bash Hook {location} {type.HookName()}: {file.Name}", writeToConsole: !type.AssociatedCommand().Silent());
                if (exitCode != 0)
                {
                    return exitCode;
                }
            }

            return 0;
        }
        
        public int FireExeHooks(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
               () => FireNamedExeHooks(type, location, args),
               () => FireUntiedExeHooks(type, location, args));
        }

        private int FireNamedExeHooks(HookType type, HookLocation location, params string[] args)
        {
            var path = GetHookFolder(location);
            FileInfo file = new FileInfo($"{path}/{type.HookName()}.exe");
            if (!file.Exists) return 0;

            WriteLine($"Firing Named Exe Hook {location} {type.HookName()}", writeToConsole: !type.AssociatedCommand().Silent());

            this.RunProcess(
                SetArgumentsOnStartInfo(
                    new ProcessStartInfo(file.FullName, string.Join(" ", args))));

            WriteLine($"Fired Named Exe Hook {location} {type.HookName()}", writeToConsole: !type.AssociatedCommand().Silent());
            return 0;
        }

        private int FireUntiedExeHooks(HookType type, HookLocation location, params string[] args)
        {
            var path = GetHookFolder(location);
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) return 0;

            foreach (var file in dir.EnumerateFiles())
            {
                if (!file.Extension.ToUpper().Equals(".EXE")) continue;
                var rawName = Path.GetFileNameWithoutExtension(file.Name);
                if (HookTypeExt.IsHookName(rawName)) continue;

                WriteLine($"Firing Untied Exe Hook {location} {type.HookName()}: {file.Name}", writeToConsole: !type.AssociatedCommand().Silent());

                var newArgs = new string[args.Length + 2];
                newArgs[0] = type.HookName();
                newArgs[1] = type.CommandString();
                Array.Copy(args, 0, newArgs, 2, args.Length);

                var exitCode = RunProcess(
                    SetArgumentsOnStartInfo(
                        new ProcessStartInfo(file.FullName, string.Join(" ", newArgs))));

                WriteLine($"Fired Untied Exe Hook {location} {type.HookName()}: {file.Name}", writeToConsole: !type.AssociatedCommand().Silent());
                if (exitCode != 0)
                {
                    return exitCode;
                }
            }

            return 0;
        }

        private ProcessStartInfo SetArgumentsOnStartInfo(ProcessStartInfo startInfo)
        {
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            return startInfo;
        }
        #endregion
    }
}
