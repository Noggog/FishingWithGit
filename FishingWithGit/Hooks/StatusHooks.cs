using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class StatusHooks : HookSet
    {
        StatusArgs args = new StatusArgs();
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Status;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Status;
        public override bool PostHookNatural => false;

        private StatusHooks(BaseWrapper wrapper)
            : base(wrapper)
        {
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new StatusHooks(wrapper);
        }
    }
}
