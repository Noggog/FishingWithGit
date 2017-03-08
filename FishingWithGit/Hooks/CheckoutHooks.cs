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
        string[] newArgs;

        private CheckoutHooks(BaseWrapper wrapper, List<string> args, int commandIndex)
            : base(wrapper)
        {
            this.newArgs = RetrieveNewArgs(args, commandIndex);
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Contains("--")
                || args.Contains("--theirs")
                || args.Contains("--ours"))
            {
                return TakeHooks.Factory(wrapper, args, commandIndex);
            }
            else
            {
                return new CheckoutHooks(wrapper, args, commandIndex);
            }
        }

        private string[] RetrieveNewArgs(List<string> args, int commandIndex)
        {
            string curSha, targetSha;
            var argsList = args.ToList();
            var trackIndex = argsList.IndexOf("--track");
            using (var repo = new Repository(Directory.GetCurrentDirectory()))
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

            return new string[]
            {
                curSha,
                targetSha
            };
        }

        private string GetTargetSha(Repository repo, string targetBranchName)
        {
            var targetBranch = repo.Branches[targetBranchName];
            if (targetBranch == null)
            {
                throw new ArgumentException($"Could not locate local branch named {targetBranchName} in repo {repo.Info.Path}");
            }
            if (targetBranch.Tip == null)
            {
                throw new ArgumentException($"Branch named {targetBranchName} did not point to a commit in repo {repo.Info.Path}.");
            }
            return targetBranch.Tip.Sha;
        } 

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Checkout, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Checkout, HookLocation.Normal, newArgs));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Checkout, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Post_Checkout, HookLocation.Normal, newArgs));
        }
    }
}
