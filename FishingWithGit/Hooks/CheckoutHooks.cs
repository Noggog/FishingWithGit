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
    public class CheckoutHooks : HookSet
    {
        CheckoutArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Checkout;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Checkout;
        public override bool PostHookNatural => true;

        private CheckoutHooks(BaseWrapper wrapper, CheckoutArgs newArgs)
            : base(wrapper)
        {
            this.args = newArgs;
        }

        public static HookSet Factory(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
        {
            if (args.Contains("--")
                || args.Contains("--theirs")
                || args.Contains("--ours"))
            {
                string currentSha;
                using (var repo = new Repository(repoDir.FullName))
                {
                    currentSha = repo.Head.Tip.Sha;
                }
                return TakeHooks.Factory(
                    wrapper,
                    currentSha,
                    args,
                    commandIndex);
            }
            else
            {
                CheckoutArgs newArgs = RetrieveNewArgs(args, repoDir, commandIndex);
                return new CheckoutHooks(wrapper, newArgs);
            }
        }

        private static CheckoutArgs RetrieveNewArgs(List<string> args, DirectoryInfo repoDir, int commandIndex)
        {
            string curSha, targetSha;
            var argsList = args.ToList();
            var trackIndex = argsList.IndexOf("--track");
            using (var repo = new Repository(repoDir.FullName))
            {
                curSha = repo.Head.Tip.Sha;
                if (trackIndex == -1)
                { // Just checking out a local branch
                    var nameIndex = CommonFunctions.Skip(args, commandIndex + 1, "-b");
                    if (nameIndex >= args.Count)
                    {
                        throw new ArgumentException("Cannot run checkout hooks, as args are invald.  No branch name was found.");
                    }
                    targetSha = GetTargetSha(repo, args[nameIndex]);
                }
                else
                { // Checking out a remote branch
                    if (args.Count <= trackIndex + 1)
                    {
                        throw new ArgumentException($"Could not locate target remote branch name.");
                    }

                    targetSha = GetTargetSha(repo, args[trackIndex + 1]);
                }
            }

            return new CheckoutArgs(
                new string[]
                {
                    curSha,
                    targetSha
                });
        }

        private static string GetTargetSha(Repository repo, string val)
        {
            if (val.Length == Constants.SHA_LENGTH)
            {
                return val;
            }

            var targetBranch = repo.Branches[val];
            if (targetBranch == null)
            {
                throw new ArgumentException($"Could not locate local branch named {val} in repo {repo.Info.Path}");
            }
            if (targetBranch.Tip == null)
            {
                throw new ArgumentException($"Branch named {val} did not point to a commit in repo {repo.Info.Path}.");
            }
            return targetBranch.Tip.Sha;
        }
    }
}
