using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TagHooks : HookSet
    {
        TagArgs args;
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Pre_Tag;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_Tag;
        public override bool PostHookNatural => false;

        private TagHooks(BaseWrapper wrapper, List<string> args)
            : base(wrapper)
        {
            this.args = new TagArgs(args.ToArray());
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new TagHooks(wrapper, args);
        }
    }
}
