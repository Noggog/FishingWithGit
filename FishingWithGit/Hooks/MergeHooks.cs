using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class MergeHooks : HookSet
    {
        MergeArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Merge;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Merge;
        public override bool PostHookNatural => true;

        public MergeHooks(BaseWrapper wrapper, List<string> args, int commandIndex)
            : base(wrapper)
        {
            this.args = new MergeArgs(
                args.ToArray(),
                commandIndex + 1);
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new MergeHooks(wrapper, args, commandIndex);
        }
    }
}

