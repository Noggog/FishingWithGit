using FishingWithGit.Common;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TakeHooks : HookSet
    {
        TakeArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Take;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Take;
        public override bool PostHookNatural => false;

        private TakeHooks(BaseWrapper wrapper, string targetSha, List<string> args, int commandIndex)
            : base(wrapper)
        {
            if (targetSha.Length != Constants.SHA_LENGTH)
            {
                throw new ArgumentException($"Sha was not correct length: {targetSha.Length}");
            }
            var argsList = args.ToList();
            commandIndex = argsList.IndexOf("--", commandIndex);
            if (commandIndex == -1)
            {
                throw new ArgumentException("Could not run take hooks.  Args invalid and missing '--'.");
            }
            var newArgs = new string[args.Count - commandIndex];
            Array.Copy(args.ToArray(), commandIndex, newArgs, 0, newArgs.Length);
            this.args = new TakeArgs()
            {
                TargetSha = targetSha,
                Items = new List<string>(newArgs)
            };
        }

        public static HookSet Factory(BaseWrapper wrapper, string targetSha, List<string> args, int commandIndex)
        {
            return new TakeHooks(wrapper, targetSha, args, commandIndex);
        }

        public static HookSet Factory(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
        {
            using (var repo = new Repository(repoDir.FullName))
            {
                return new TakeHooks(wrapper, repo.Head.Tip.Sha, args, commandIndex);
            }

        }
    }
}
