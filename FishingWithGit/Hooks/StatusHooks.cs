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

        private StatusHooks(BaseWrapper wrapper)
            : base(wrapper)
        {
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new StatusHooks(wrapper);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Status, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Status, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Status, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Post_Status, HookLocation.Normal, args.ToArray()));
        }
    }
}
