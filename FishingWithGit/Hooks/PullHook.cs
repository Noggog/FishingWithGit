using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class PullHooks : HookSet
    {
        PullArgs args;

        private PullHooks(BaseWrapper wrapper, string currentSha, string targetSha)
            : base(wrapper)
        {
            this.args = new PullArgs(
                new string[]
                {
                    currentSha,
                    targetSha
                });
        }

        public static HookSet Factory(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
        {
            if (args.Count < commandIndex + 2)
            {
                throw new ArgumentException("Missing origin or branch name arguments.");
            }
            var originStr = args[commandIndex + 1];
            var branchStr = args[commandIndex + 2];
            string currentSha, targetSha;
            using (var repo = new Repository(repoDir.FullName))
            {
                currentSha = repo.Head.Tip.Sha;
                var targetBranchStr = $"{originStr}/{branchStr}";
                var branch = repo.Branches[targetBranchStr];
                if (branch == null)
                {
                    throw new ArgumentException($"Target branch {targetBranchStr} did not exist.");
                }
                targetSha = branch.Tip.Sha;
            }
            return new PullHooks(wrapper, currentSha, targetSha);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Pull, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_Pull, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Pull, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Post_Pull, HookLocation.Normal, args.ToArray()));
        }
    }
}
