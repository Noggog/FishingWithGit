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

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Merge, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_Merge, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Merge, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Post_Merge, HookLocation.Normal, args.ToArray()));
        }
    }
}

