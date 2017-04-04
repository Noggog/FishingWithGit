using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class BranchHooks : HookSet
    {
        BranchArgs args;

        private BranchHooks(BaseWrapper wrapper, BranchArgs args)
            : base(wrapper)
        {
            this.args = args;
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count < commandIndex + 1)
            {
                throw new ArgumentException("Missing branch name argument.");
            }
            return new BranchHooks(
                wrapper,
                new BranchArgs()
                {
                    TargetBranch = args[commandIndex + 1],
                    Deleting = args.Contains("-D")
                });
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Pull, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Pull, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Pull, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Post_Pull, HookLocation.Normal, args.ToArray()));
        }
    }
}
