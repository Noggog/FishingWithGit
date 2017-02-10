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
            string cmd = GetMainCommand(args, out index);
            if (index == -1)
            {
                wrapper.WriteLine("No command found.");
                return null;
            }
            else
            {
                wrapper.WriteLine("Command: " + cmd, writeToConsole: true);
            }
            switch (cmd)
            {
                case "checkout":
                    return CheckoutHooks(args);
                case "rebase":
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

        public HookPair CheckoutHooks(string[] args)
        {
            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireHook(HookType.Pre_Checkout, HookLocation.InRepo);
                    FireHook(HookType.Pre_Checkout, HookLocation.Normal);
                },
                PostCommand = () =>
                {
                    FireHook(HookType.Post_Checkout, HookLocation.InRepo);
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
            FileInfo file = new FileInfo(path + "/" + commandType.HookName());
            wrapper.WriteLine("Looking for hook file " + file.FullName, writeToConsole: true);
            if (!file.Exists) return;

            wrapper.WriteLine("Firing " + commandType.HookName() + " , " + hookType, writeToConsole: true);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Join(" ", args);
            startInfo.FileName = file.FullName;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            startInfo.UseShellExecute = false;
            using (Process p = Process.Start(startInfo))
            {
                p.WaitForExit();
            }
        }
    }
}
