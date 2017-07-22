using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitHooks : HookSet
    {
        CommitArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Commit;
        public override bool PreHookNatural => true;
        public override HookType PostType => HookType.Post_Commit;
        public override bool PostHookNatural => true;

        private CommitHooks(BaseWrapper wrapper, List<string> args)
            : base(wrapper)
        {
            this.args = new CommitArgs(args.ToArray());
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new CommitHooks(wrapper, args);
        }
    }
}
