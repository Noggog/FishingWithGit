using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class PushHooks : HookSet
    {
        PushArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Push;
        public override bool PreHookNatural => true;
        public override HookType PostType => HookType.Post_Push;
        public override bool PostHookNatural => false;

        private PushHooks(BaseWrapper wrapper, List<string> args)
            : base(wrapper)
        {
            this.args = new PushArgs(args.ToArray());
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new PushHooks(wrapper, args);
        }
    }
}
