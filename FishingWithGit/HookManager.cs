﻿using LibGit2Sharp;
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
            CommandType type;
            if (!Enum.TryParse<CommandType>(cmdStr, out type)) return null;

            wrapper.WriteLine($"Command: {cmdStr}", writeToConsole: true);
            switch (type)
            {
                case CommandType.checkout:
                    return CheckoutHooks(args, index);
                case CommandType.rebase:
                    return RebaseHooks(args);
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
                    // Normal already exists.
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
                    // Normal already exists.
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Rebase, HookLocation.InRepo);
                    FireHook(HookType.Post_Rebase, HookLocation.Normal);
                }
            };
        }

        public void FireHook(HookType commandType, HookLocation hookType, params string[] args)
        {
            var path = Directory.GetCurrentDirectory();
            if (hookType == HookLocation.Normal)
            {
                path += "/.git";
            }
            path += "/hooks";
            FileInfo file = new FileInfo($"{path}/{commandType.HookName()}");
            wrapper.WriteLine("Looking for hook file " + file.FullName);
            if (!file.Exists) return;

            wrapper.WriteLine($"Firing {hookType} {commandType.HookName()}", writeToConsole: true);

            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + file.FullName + " " + string.Join(" ", args));
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            this.wrapper.RunProcess(processInfo);
        }
    }
}
