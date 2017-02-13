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
    class HookManager
    {
        BaseWrapper wrapper;
        public HookManager(BaseWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        public HookPair GetHook(string[] args)
        {
            int index;
            string cmdStr = GetMainCommand(args, out index);
            if (index == -1)
            {
                wrapper.WriteLine("No command found.");
                return null;
            }
            CommandType command;
            if (!CommandTypeExt.TryParse(cmdStr, out command)) return null;

            wrapper.WriteLine($"Command: {cmdStr}", writeToConsole: !command.Silent());
            switch (command)
            {
                case CommandType.checkout:
                    return CheckoutHooks(args, index);
                //case CommandType.rebase:
                //    return RebaseHooks(args);
                case CommandType.reset:
                    return ResetHooks(args, index);
                case CommandType.commitmsg:
                    return CommitMsgHooks(args, index);
                case CommandType.commit:
                    return CommitHooks(args, index);
                case CommandType.status:
                    return StatusHooks(args);
                default:
                    return null;
            }
        }

        private string GetMainCommand(string[] args, out int index)
        {
            for (int i = 0; i < args.Length; i++)
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

        public HookPair CheckoutHooks(string[] args, int commandIndex)
        {
            if (args.Length <= commandIndex + 1)
            {
                throw new ArgumentException("Cannot run checkout hooks, as args are invald.  No content was found after checkout command.");
            }

            string curSha, targetSha;
            using (var repo = new Repository(Directory.GetCurrentDirectory()))
            {
                curSha = repo.Head.Tip.Sha;
                targetSha = repo.Branches[args[commandIndex + 1]].Tip.Sha;
            }

            var newArgs = new string[]
            {
                curSha,
                targetSha
            };

            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Pre_Checkout, HookLocation.InRepo, newArgs);
                    FireHook(HookType.Pre_Checkout, HookLocation.Normal, newArgs);
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Checkout, HookLocation.InRepo, newArgs);
                    FireExeHooks(HookType.Post_Checkout, HookLocation.Normal, newArgs);
                }
            };
        }

        public HookPair RebaseHooks(string[] args)
        {
            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Pre_Rebase, HookLocation.InRepo);
                    FireExeHooks(HookType.Pre_Rebase, HookLocation.Normal);
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Rebase, HookLocation.InRepo);
                    FireHook(HookType.Post_Rebase, HookLocation.Normal);
                }
            };
        }

        public HookPair ResetHooks(string[] args, int commandIndex)
        {
            string sha = null;
            string type = null;
            for (int i = commandIndex + 1; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-")
                    && args[i].Length == 40)
                {
                    sha = args[i];
                }
                if (args[i].StartsWith("--"))
                {
                    type = args[i].Substring(2);
                }
            }

            if (sha == null)
            {
                throw new ArgumentException("Cannot run reset hooks, as args are invald.  No sha could be found.");
            }

            if (type == null)
            {
                throw new ArgumentException("Cannot run reset hooks, as args are invald.  No type could be found.");
            }

            switch (type)
            {
                case "soft":
                case "mixed":
                case "hard":
                    break;
                default:
                    throw new ArgumentException($"Cannot run reset hooks, as args are invalid.  Type was invalid: {type}");
            }

            string curBranch;
            using (var repo = new Repository(Directory.GetCurrentDirectory()))
            {
                curBranch = repo.Head.FriendlyName;
            }

            var newArgs = new string[]
            {
                curBranch,
                sha,
                type
            };

            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Pre_Reset, HookLocation.InRepo, newArgs);
                    FireHook(HookType.Pre_Reset, HookLocation.Normal, newArgs);
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Reset, HookLocation.InRepo, newArgs);
                    FireHook(HookType.Post_Reset, HookLocation.Normal, newArgs);
                }
            };
        }

        public HookPair StatusHooks(string[] args)
        {
            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Pre_Status, HookLocation.InRepo, args);
                    FireHook(HookType.Pre_Status, HookLocation.Normal, args);
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Status, HookLocation.InRepo, args);
                    FireHook(HookType.Post_Status, HookLocation.Normal, args);
                }
            };
        }

        public HookPair CommitMsgHooks(string[] args, int commandIndex)
        {
            if (args.Length <= commandIndex + 1)
            {
                throw new ArgumentException("Cannot run checkout hooks, as args are invald.  No content was found after checkout command.");
            }

            var newArgs = new string[]
            {
                args[commandIndex + 1]
            };

            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Commit_Msg, HookLocation.InRepo, newArgs);
                    FireExeHooks(HookType.Commit_Msg, HookLocation.Normal, newArgs);
                }
            };
        }

        public HookPair CommitHooks(string[] args, int commandIndex)
        {
            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Pre_Commit, HookLocation.InRepo);
                    FireExeHooks(HookType.Pre_Commit, HookLocation.Normal);
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Commit, HookLocation.InRepo);
                    FireExeHooks(HookType.Post_Commit, HookLocation.Normal);
                }
            };
        }

        public void FireHook(HookType type, HookLocation location, params string[] args)
        {
            FireBashHook(type, location, args);
            FireExeHooks(type, location, args);
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

        private void FireBashHook(HookType type, HookLocation location, params string[] args)
        {
            var path = GetHookFolder(location);
            FileInfo file = new FileInfo($"{path}/{type.HookName()}");
            wrapper.WriteLine("Looking for hook file " + file.FullName);
            if (!file.Exists) return;

            wrapper.WriteLine($"Firing Bash Hook {location} {type.HookName()}", writeToConsole: !type.AssociatedCommand().Silent());
            
            this.wrapper.RunProcess(
                SetArgumentsOnStartInfo(
                    new ProcessStartInfo(file.FullName, string.Join(" ", args))));

            wrapper.WriteLine($"Fired Bash Hook {location} {type.HookName()}", writeToConsole: !type.AssociatedCommand().Silent());
        }

        private void FireExeHooks(HookType type, HookLocation location, params string[] args)
        {
            var path = GetHookFolder(location);
            DirectoryInfo dir = new DirectoryInfo(path);
            wrapper.WriteLine("Looking for dir " + path);
            if (!dir.Exists) return;

            wrapper.WriteLine("Looking for exe files to run.");
            foreach (var file in dir.EnumerateFiles())
            {
                wrapper.WriteLine("Looking at " + file.Name);
                if (!file.Extension.ToUpper().Equals(".EXE")) continue;
                var rawName = Path.GetFileNameWithoutExtension(file.Name);
                if (HookTypeExt.IsCommandString(rawName)
                    && !rawName.Equals(type.CommandString())) continue;

                wrapper.WriteLine($"Firing Exe Hook {location} {type.HookName()}: {file.Name}", writeToConsole: !type.AssociatedCommand().Silent());

                var newArgs = new string[args.Length + 2];
                newArgs[0] = type.HookName();
                newArgs[1] = type.CommandString();
                Array.Copy(args, 0, newArgs, 2, args.Length);

                this.wrapper.RunProcess(
                    SetArgumentsOnStartInfo(
                        new ProcessStartInfo(file.FullName, string.Join(" ", newArgs))));

                wrapper.WriteLine($"Fired Exe Hook {location} {type.HookName()}: {file.Name}", writeToConsole: !type.AssociatedCommand().Silent());

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
    }
}
