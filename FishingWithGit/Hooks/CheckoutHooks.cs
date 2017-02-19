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

        private CheckoutHooks(BaseWrapper wrapper, string[] args, int commandIndex, string targetBranchName)
            : base(wrapper)
        {
            string curSha, targetSha;
            using (var repo = new Repository(Directory.GetCurrentDirectory()))
            {
                curSha = repo.Head.Tip.Sha;
                var targetBranch = repo.Branches[targetBranchName];
                if (targetBranch == null)
                {
                    throw new ArgumentException($"Could not locate branch named {targetBranchName} in repo {repo.Info.Path}");
                }
                if (targetBranch.Tip == null)
                {
                    throw new ArgumentException($"Branch named {targetBranchName} did not point to a commit in repo {repo.Info.Path}.");
                }
                targetSha = targetBranch.Tip.Sha;
            }

            newArgs = new string[]
            {
                curSha,
                targetSha
            };
        }

        public static HookSet Factory(BaseWrapper wrapper, string[] args, int commandIndex)
        {
            if (args.Contains("--")
                || args.Contains("--theirs")
                || args.Contains("--ours"))
            {
                return TakeHooks.Factory(wrapper, args, commandIndex);
            }
            else
            {
                var nameIndex = CommonFunctions.Skip(args, commandIndex + 1, "-b");
                if (nameIndex >= args.Length)
                {
                    throw new ArgumentException("Cannot run checkout hooks, as args are invald.  No branch name was found.");
                }
                return new CheckoutHooks(wrapper, args, commandIndex, args[nameIndex]);
            }
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Pre_Checkout, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireHook(HookType.Pre_Checkout, HookLocation.Normal, newArgs));
        }

        public override int PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Post_Checkout, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireExeHooks(HookType.Post_Checkout, HookLocation.Normal, newArgs));
        }
    }
}
