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
    public class ResetHooks : HookSet
    {
        ResetArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Reset;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Reset;
        public override bool PostHookNatural => false;

        private ResetHooks(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
            : base(wrapper)
        {
            string targetSha = null;
            string type = null;
            for (int i = commandIndex + 1; i < args.Count; i++)
            {
                if (!args[i].StartsWith("-")
                    && args[i].Length == Constants.SHA_LENGTH)
                {
                    targetSha = args[i];
                }
                if (args[i].StartsWith("--"))
                {
                    type = args[i].Substring(2);
                }
            }

            if (targetSha == null)
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

            string curBranch, curSha;
            using (var repo = new Repository(repoDir.FullName))
            {
                curBranch = repo.Head.FriendlyName;
                curSha = repo.Head.Tip.Sha;
            }

            this.args = new ResetArgs(
                new string[]
                {
                    curBranch,
                    curSha,
                    targetSha,
                    type
                });
        }

        public static HookSet Factory(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
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
                return new ResetHooks(wrapper, repoDir, args, commandIndex);
            }
            else
            {
                var splitIndex = args.IndexOf("--");
                if (splitIndex == -1)
                {
                    throw new ArgumentException("Item split was not found: --");
                }
                var sha = args[splitIndex - 1];
                if (sha.Length != Constants.SHA_LENGTH)
                {
                    return TakeHooks.Factory(wrapper, repoDir, args, commandIndex);
                }
                return TakeHooks.Factory(wrapper, sha, args, commandIndex);
            }
        }
    }
}
