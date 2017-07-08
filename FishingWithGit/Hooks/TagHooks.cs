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

        private TagHooks(BaseWrapper wrapper, List<string> args)
            : base(wrapper)
        {
            this.args = new TagArgs(args.ToArray());
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new TagHooks(wrapper, args);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Tag, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Tag, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Tag, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Post_Tag, HookLocation.Normal, args.ToArray()));
        }
    }
}
