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
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Pull;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Pull;
        public override bool PostHookNatural => false;

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
    }
}
