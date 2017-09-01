using FishingWithGit.Common;
using LibGit2Sharp;
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
        CommandType commandType;
        int commandIndex;
        List<string> args;
        Stopwatch overallSw;
        TimeSpan actualCommandSpan;
        TimeSpan preHookSpan;
        TimeSpan postHookSpan;
        public Logger Logger = new Logger("FishingWithGit");

        public BaseWrapper(string[] args)
        {
            this.args = args.ToList();
            this.Logger.ShouldLogToFile = Settings.Instance.ShouldLog;
            this.Logger.WipeLogsOlderThanDays = Settings.Instance.WipeLogsOlderThanDays;
        }

        public async Task<int> Wrap(Stopwatch sw)
        {
            overallSw = sw;
            try
            {
                if (HandleIsFishing()) return 0;
                overallSw.Start();
                this.Logger.WriteLine(DateTime.Now.ToString());
                this.Logger.WriteLine($"Settings loaded from: {Settings.Instance.PathLoadedFrom}");
                this.Logger.WriteLine("Arguments:");
                if (Settings.Instance.PrintSeparateArgs)
                {
                    foreach (var arg in args)
                    {
                        this.Logger.WriteLine(arg);
                    }
                }
                else
                {
                    this.Logger.WriteLine($"  {string.Join(" ", args)}");
                }
                this.Logger.WriteLine("");
                GetCommandInfo();
                this.Logger.WriteLine($"Command: {commandType.ToString()}", writeToConsole: null);
                ProcessArgs();
                GetMainCommand(out commandIndex);
                this.Logger.WriteLine($"Working directory {Directory.GetCurrentDirectory()}.  Elapsed: {overallSw.ElapsedMilliseconds}ms");
                if (!GetStartInfo(out var startInfo)) return -1;
                this.Logger.WriteLine($"Getting hook.  Elapsed: {overallSw.ElapsedMilliseconds}ms");
                HookSet hook = GetHook();
                this.Logger.ConsoleSilent = hook?.Args.Silent ?? true;
                this.Logger.WriteLine($"Starting logs.  Elapsed: {overallSw.ElapsedMilliseconds}ms");
                this.Logger.ActivateAndFlushLogging();
                this.Logger.WriteLine($"Silent?: {this.Logger.ConsoleSilent}.  Elapsed: {overallSw.ElapsedMilliseconds}ms");
                if (Settings.Instance.FireHookLogic)
                {
                    if (hook == null)
                    {
                        this.Logger.WriteLine("No hooks for this command.");
                    }
                    else
                    {
                        this.Logger.WriteLine("Firing prehooks.");
                        var startTime = overallSw.Elapsed;
                        int? hookExitCode = await hook.PreCommand();
                        preHookSpan = overallSw.Elapsed - startTime;
                        this.Logger.WriteLine("Fired prehooks.");
                        if (0 != (hookExitCode ?? 0))
                        {
                            this.Logger.WriteLine($"Exiting early because of hook failure ({hookExitCode})", writeToConsole: true);
                            return hookExitCode.Value;
                        }
                    }
                }
                else
                {
                    this.Logger.WriteLine("Fire hook logic is off.");
                }
                int exitCode;
                if (args.Contains("-NO_PASSING_FISH"))
                {
                    exitCode = 0;
                }
                else
                {
                    exitCode = await RunActualGit(startInfo);
                }
                if (exitCode != 0)
                {
                    return exitCode;
                }
                if (Settings.Instance.FireHookLogic
                    && hook != null)
                {
                    this.Logger.WriteLine("Firing posthooks.");
                    var startTime = overallSw.Elapsed;
                    int? hookExitCode = await hook.PostCommand();
                    postHookSpan = overallSw.Elapsed - startTime;
                    this.Logger.WriteLine("Fired posthooks.");
                    if (0 != (hookExitCode ?? 0))
                    {
                        this.Logger.WriteLine($"Exiting early because of hook failure ({hookExitCode})", writeToConsole: true);
                        return hookExitCode.Value;
                    }
                }
                return exitCode;
            }
            catch (Exception ex)
            {
                this.Logger.ShouldLogToFile = true;
                this.Logger.ActivateAndFlushLogging();
                this.Logger.WriteLine("An error occurred!!!: " + ex.Message, writeToConsole: true);
                this.Logger.WriteLine(ex.ToString(), writeToConsole: true);
                throw;
            }
            finally
            {
                overallSw.Stop();
                this.Logger.WriteLine($"Command overall took {overallSw.ElapsedMilliseconds}ms.");
                if (preHookSpan.TotalMilliseconds > 0)
                {
                    this.Logger.WriteLine($"Pre hook took {preHookSpan.TotalMilliseconds}ms.");
                }
                if (actualCommandSpan.TotalMilliseconds > 0)
                {
                    this.Logger.WriteLine($"Actual git command took {actualCommandSpan.TotalMilliseconds}ms.");
                }
                if (postHookSpan.TotalMilliseconds > 0)
                {
                    this.Logger.WriteLine($"Post hook took {postHookSpan.TotalMilliseconds}ms.");
                }
                this.Logger.WriteLine($"Fishing With Git took {overallSw.ElapsedMilliseconds - preHookSpan.TotalMilliseconds - actualCommandSpan.TotalMilliseconds - postHookSpan.TotalMilliseconds}ms.");
                this.Logger.WriteLine("--------------------------------------------------------------------------------------------------------- Fishing With Git call done.");
                if (this.Logger.ShouldLogToFile)
                {
                    this.Logger.LogResults();
                }
            }
        }

        bool HandleIsFishing()
        {
            if (args.Count != 1) return false;
            if (string.Equals(args[0], Constants.IS_FISHING_CMD))
            {
                System.Console.WriteLine(Constants.IS_FISHING_RESP);
                this.Logger.WriteLine("Responded to IsFishing check.", writeToConsole: false);
                return true;
            }
            return false;
        }

        void GetCommandInfo()
        {
            string cmdStr = GetMainCommand(out commandIndex);
            if (commandIndex == -1)
            {
                commandType = CommandType.unknown;
                return;
            }
            if (!CommandTypeExt.TryParse(cmdStr, out commandType))
            {
                commandType = CommandType.unknown;
                this.Logger.WriteLine("Unknown command: " + cmdStr);
            }
        }

        async Task<int> RunActualGit(ProcessStartInfo startInfo)
        {
            this.Logger.WriteLine($"Running actual command.  Elapsed: {overallSw.ElapsedMilliseconds}ms");
            var startTime = overallSw.Elapsed;
            int exitCode = await RunProcess(startInfo, hookIO: true);
            var endTime = overallSw.Elapsed;
            actualCommandSpan = endTime - startTime;
            return exitCode;
        }

        bool GetStartInfo(out ProcessStartInfo startInfo)
        {
            startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Join(" ", args);
            if (!GetGitPath(out var gitFile))
            {
                this.Logger.WriteLine("Could not find git install.");
                return false;
            }
            var exePath = gitFile.FullName;
            this.Logger.WriteLine($"Target exe {exePath}ms.  Elapsed {overallSw.ElapsedMilliseconds}ms");
            startInfo.FileName = exePath;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            return true;
        }

        public bool GetGitPath(out FileInfo gitFile)
        {
            // Return if overridden
            if (!string.IsNullOrWhiteSpace(Settings.Instance.RealGitProgramPathOverride))
            {
                gitFile = new FileInfo(Settings.Instance.RealGitProgramPathOverride);
                return true;
            }

            // Return if cached
            if (!string.IsNullOrWhiteSpace(Settings.Instance.RealGitProgramPath))
            {
                gitFile = new FileInfo(Settings.Instance.RealGitProgramPath);
                if (gitFile.Exists) return true;
            }

            // Query PATH for viable git installs
            this.Logger.WriteLine($"Querying PATH. Elapsed: {overallSw.ElapsedMilliseconds}ms", writeToConsole: false);
            string pathStr = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            string[] paths = pathStr.Split(';');
            foreach (var path in paths)
            {
                try
                {
                    var dir = new DirectoryInfo(path);
                    if (!dir.Name.Equals("cmd")) continue;
                    if (!dir.Exists) continue;
                    foreach (var file in dir.EnumerateFiles())
                    {
                        if (!file.Name.Equals("git.exe")) continue;
                        if (Utility.TestIfFishingEXE(file.FullName)) continue;

                        // Probably a real git install
                        this.Logger.WriteLine($"Found git install at {file.FullName}. Elapsed: {overallSw.ElapsedMilliseconds}ms", writeToConsole: false);
                        gitFile = file;
                        Settings.Instance.RealGitProgramPath = file.FullName;
                        try
                        {
                            if (!Settings.Instance.SaveSettings())
                            {
                                this.Logger.WriteLine(
                                    $"Could not save settings to {Settings.Instance.PathLoadedFrom}",
                                    error: false);
                            }
                            else
                            {
                                this.Logger.WriteLine($"Saved settings to {Settings.Instance.PathLoadedFrom}");
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Logger.WriteLine(
                                $"Error saving settings. {ex}",
                                error: false);
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.WriteLine(
                        $"Error loading PATH {path}. {ex}",
                        error: true,
                        writeToConsole: false);
                }
            }

            gitFile = default(FileInfo);
            return false;
        }

        public async Task<int> RunProcess(ProcessStartInfo startInfo, bool hookIO)
        {
            FileInfo file = new FileInfo(startInfo.FileName);
            if (!file.Exists)
            {
                throw new ArgumentException("File does not exist: " + file.FullName);
            }
            using (var process = Process.Start(startInfo))
            {
                List<Task> tasks = new List<Task>(3);
                tasks.Add(Task.Run(() => process.WaitForExit()));
                if (hookIO)
                {
                    await TypicalHookIO(process);
                }
                var task = Task.WhenAll(tasks);
                if (task != await Task.WhenAny(task, Task.Delay(Settings.Instance.ProcessTimeoutWarning)))
                {
                    this.Logger.WriteLine("Process taking a long time: " + startInfo.FileName + " " + startInfo.Arguments, error: true);
                }
                await task;

                if (process.ExitCode != 0
                    && !hookIO)
                {
                    await TypicalHookIO(process);
                }

                return process.ExitCode;
            }
        }

        private Task TypicalHookIO(Process proccess)
        {
            bool firstStandard = true;
            bool firstErr = true;
            return HookIO(
                proccess,
                standard: (result) =>
                {
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        if (firstStandard)
                        {
                            this.Logger.WriteLine("--------- Standard Output :");
                            firstStandard = false;
                        }
                        this.Logger.WriteLine(result, writeToConsole: true);
                    }
                },
                err: (result) =>
                {
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        if (firstErr)
                        {
                            this.Logger.WriteLine("--------- Standard Error :", error: true);
                            firstErr = false;
                        }
                        this.Logger.WriteLine(result, writeToConsole: true, error: true);
                    }
                });
        }

        private Task HookIO(Process process, Action<string> standard, Action<string> err)
        {
            return Task.WhenAll(
                Task.Run(() =>
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        standard?.Invoke(reader.ReadToEnd());
                    }
                }),
                Task.Run(() =>
                {
                    using (StreamReader reader = process.StandardError)
                    {
                        err?.Invoke(reader.ReadToEnd());
                    }
                }));
        }

        private void ProcessArgs()
        {
            try
            {
                ArgProcessor.Process(this.commandType, args);
            }
            finally
            {
                this.Logger.WriteLine("Arguments going in:");
                this.Logger.WriteLine($"  {string.Join(" ", args)}");
                this.Logger.WriteLine("");
            }
        }

        #region Getting Hooks
        public HookSet GetHook()
        {
            switch (commandType)
            {
                case CommandType.checkout:
                    return CheckoutHooks.Factory(
                        this,
                        new DirectoryInfo(Directory.GetCurrentDirectory()),
                        args,
                        commandIndex);
                case CommandType.rebase:
                    return RebaseHooks.Factory(
                        this,
                        new DirectoryInfo(Directory.GetCurrentDirectory()),
                        args,
                        commandIndex);
                case CommandType.reset:
                    return ResetHooks.Factory(
                        this,
                        new DirectoryInfo(Directory.GetCurrentDirectory()),
                        args,
                        commandIndex);
                case CommandType.commitmsg:
                    return CommitMsgHooks.Factory(this, args, commandIndex);
                case CommandType.commit:
                    return CommitHooks.Factory(this, args, commandIndex);
                case CommandType.status:
                    return StatusHooks.Factory(this, args, commandIndex);
                case CommandType.cherry:
                    return CherryPickHook.Factory(this, args, commandIndex);
                case CommandType.merge:
                    return MergeHooks.Factory(this, args, commandIndex);
                case CommandType.pull:
                    return PullHooks.Factory(
                        this,
                        new DirectoryInfo(Directory.GetCurrentDirectory()),
                        args,
                        commandIndex);
                case CommandType.branch:
                    return BranchHooks.Factory(this, args, commandIndex);
                case CommandType.tag:
                    return TagHooks.Factory(this, args, commandIndex);
                case CommandType.push:
                    return PushHooks.Factory(this, args, commandIndex);
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
        public Task<int> FireHooks(HookType type, bool natural, params string[] args)
        {
            if (natural)
            {
                return CommonFunctions.RunCommands(
                    () => this.FireAllHooks(type, HookLocation.InRepo, args),
                    () => this.FireUnnaturalHooks(type, HookLocation.Normal, args));
            }
            else
            {
                return CommonFunctions.RunCommands(
                    () => this.FireAllHooks(type, HookLocation.InRepo, args),
                    () => this.FireAllHooks(type, HookLocation.Normal, args));
            }
        }

        private Task<int> FireAllHooks(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
                () => FireBashHook(type, location, args),
                () => FireExeHooks(type, location, args));
        }

        private Task<int> FireUnnaturalHooks(HookType type, HookLocation location, params string[] args)
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

        private Task<int> FireBashHook(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
               () => FireNamedBashHook(type, location, args),
               () => FireUntiedBashHook(type, location, args));
        }

        private async Task<int> FireNamedBashHook(HookType type, HookLocation location, params string[] args)
        {
            if (!CheckLocationFiringSwitch(location)) return 0;
            var path = GetHookFolder(location);
            FileInfo file = new FileInfo($"{path}/{type.HookName()}");
            if (!file.Exists) return 0;

            this.Logger.WriteLine($"Firing Named Bash Hook {location} {type.HookName()} with args: {string.Join(" ", args)}", writeToConsole: null);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var exitCode = await RunProcess(
                SetArgumentsOnStartInfo(
                    new ProcessStartInfo(file.FullName, string.Join(" ", args))),
                hookIO: !this.Logger.ConsoleSilent);

            sw.Stop();
            this.Logger.WriteLine($"Fired Named Bash Hook {location} {type.HookName()}.  Took {sw.ElapsedMilliseconds}ms", writeToConsole: null);
            return exitCode;
        }

        private async Task<int> FireUntiedBashHook(HookType type, HookLocation location, params string[] args)
        {
            if (!CheckLocationFiringSwitch(location)) return 0;
            var path = GetHookFolder(location);
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists) return 0;

            foreach (var file in dir.EnumerateFiles())
            {
                if (!file.Extension.ToUpper().Equals(string.Empty)) continue;
                var rawName = Path.GetFileNameWithoutExtension(file.Name);
                if (HookTypeExt.IsHookName(rawName)) continue;

                this.Logger.WriteLine($"Firing Untied Bash Hook {location} {type.HookName()} {file.Name} with args: {string.Join(" ", args)}", writeToConsole: null);
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var exitCode = await this.RunProcess(
                    SetArgumentsOnStartInfo(
                        new ProcessStartInfo(file.FullName, string.Join(" ", args))),
                    hookIO: !this.Logger.ConsoleSilent);

                sw.Stop();
                this.Logger.WriteLine($"Fired Untied Bash Hook {location} {type.HookName()} {file.Name}.  Took {sw.ElapsedMilliseconds}ms", writeToConsole: null);
                if (exitCode != 0)
                {
                    return exitCode;
                }
            }

            return 0;
        }

        public Task<int> FireExeHooks(HookType type, HookLocation location, params string[] args)
        {
            return CommonFunctions.RunCommands(
               () => FireNamedExeHooks(type, location, args),
               () => FireUntiedExeHooks(type, location, args));
        }

        private async Task<int> FireNamedExeHooks(HookType type, HookLocation location, params string[] args)
        {
            if (!CheckLocationFiringSwitch(location)) return 0;
            var path = GetHookFolder(location);
            FileInfo file = new FileInfo($"{path}/{type.HookName()}.exe");
            if (!file.Exists) return 0;

            this.Logger.WriteLine($"Firing Named Exe Hook {location} {type.HookName()} with args: {string.Join(" ", args)}", writeToConsole: null);
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var exitCode = await this.RunProcess(
                SetArgumentsOnStartInfo(
                    new ProcessStartInfo(file.FullName, string.Join(" ", args))),
                hookIO: !this.Logger.ConsoleSilent);

            sw.Stop();
            this.Logger.WriteLine($"Fired Named Exe Hook {location} {type.HookName()}.  Took {sw.ElapsedMilliseconds}ms", writeToConsole: null);
            return exitCode;
        }

        private async Task<int> FireUntiedExeHooks(HookType type, HookLocation location, params string[] args)
        {
            if (!CheckLocationFiringSwitch(location)) return 0;
            var path = GetHookFolder(location);
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                foreach (var file in dir.EnumerateFiles())
                {
                    if (!file.Extension.ToUpper().Equals(".EXE")) continue;
                    var rawName = Path.GetFileNameWithoutExtension(file.Name);
                    if (HookTypeExt.IsHookName(rawName)) continue;

                    this.Logger.WriteLine($"Firing Untied Exe Hook {location} {type.HookName()} {file.Name} with args: {string.Join(" ", args)}", writeToConsole: null);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    var newArgs = new string[args.Length + 2];
                    newArgs[0] = type.HookName();
                    newArgs[1] = type.CommandString();
                    Array.Copy(args, 0, newArgs, 2, args.Length);

                    var exitCode = await RunProcess(
                        SetArgumentsOnStartInfo(
                            new ProcessStartInfo(file.FullName, string.Join(" ", newArgs))),
                        hookIO: !this.Logger.ConsoleSilent);

                    sw.Stop();
                    this.Logger.WriteLine($"Fired Untied Exe Hook {location} {type.HookName()} {file.Name}.  Took {sw.ElapsedMilliseconds}ms", writeToConsole: null);
                    if (exitCode != 0)
                    {
                        return exitCode;
                    }
                }
            }

            return 0;
        }

        public async Task<int> FireMassHooks(HookType type, params string[] args)
        {
            if (!CheckLocationFiringSwitch(HookLocation.Mass)) return 0;
            var exe = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);
            var massHookDir = new DirectoryInfo(Path.Combine(exe.Directory.FullName, Settings.Instance.MassHookFolder));
            if (!massHookDir.Exists)
            {
                this.Logger.WriteLine($"No mass hook folder at {massHookDir.FullName}.", writeToConsole: null);
                return 0;
            }
            foreach (var file in massHookDir.EnumerateFiles()
                .Union(massHookDir.EnumerateDirectories().SelectMany((d) => d.EnumerateFiles())))
            {
                if (!file.Extension.ToUpper().Equals(".EXE")) continue;
                var rawName = Path.GetFileNameWithoutExtension(file.Name);
                if (HookTypeExt.IsHookName(rawName)) continue;

                this.Logger.WriteLine($"Firing Mass Exe Hook {type.HookName()} {file.Name} with args: {string.Join(" ", args)}", writeToConsole: null);
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var newArgs = new string[args.Length + 2];
                newArgs[0] = type.HookName();
                newArgs[1] = type.CommandString();
                Array.Copy(args, 0, newArgs, 2, args.Length);

                var exitCode = await RunProcess(
                    SetArgumentsOnStartInfo(
                        new ProcessStartInfo(file.FullName, string.Join(" ", newArgs))),
                    hookIO: !this.Logger.ConsoleSilent);

                sw.Stop();
                this.Logger.WriteLine($"Fired Mass Exe Hook {type.HookName()} {file.Name}.  Took {sw.ElapsedMilliseconds}ms", writeToConsole: null);
                if (exitCode != 0)
                {
                    return exitCode;
                }
            }

            return 0;
        }

        private bool CheckLocationFiringSwitch(HookLocation loc)
        {
            switch (loc)
            {
                case HookLocation.Normal:
                    return Settings.Instance.RunNormalFolderHooks;
                case HookLocation.InRepo:
                    return Settings.Instance.RunInRepoHooks;
                case HookLocation.Mass:
                    return Settings.Instance.RunMassHooks;
                default:
                    throw new NotImplementedException();
            }
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
