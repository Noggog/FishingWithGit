using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class ResetHooks : HookSet
    {
        string[] newArgs;

        private ResetHooks(BaseWrapper wrapper, List<string> args, int commandIndex)
            : base(wrapper)
        {
            string sha = null;
            string type = null;
            for (int i = commandIndex + 1; i < args.Count; i++)
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

            newArgs = new string[]
            {
                curBranch,
                sha,
                type
            };
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count <= commandIndex + 1)
            {
                throw new ArgumentException("Cannot run reset hooks, as args are invald.  No content was found after checkout command.");
            }
            var argsList = args.ToList();
            var extraCommand = args[commandIndex + 1];
            if (args.Contains("--soft")
                || args.Contains("--mixed")
                || args.Contains("--hard"))
            {
                return new ResetHooks(wrapper, args, commandIndex);
            }
            else
            {
                return TakeHooks.Factory(wrapper, args, commandIndex);
            }
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Reset, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Reset, HookLocation.Normal, newArgs));
        }

        public override int PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Reset, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireAllHooks(HookType.Post_Reset, HookLocation.Normal, newArgs));
        }
    }
}
