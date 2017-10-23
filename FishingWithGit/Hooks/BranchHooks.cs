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
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Branch;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Branch;
        public override bool PostHookNatural => false;

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
                new BranchArgs(args.ToArray()));
        }
    }
}
