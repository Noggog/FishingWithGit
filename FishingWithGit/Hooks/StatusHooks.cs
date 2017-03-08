using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class StatusHooks : HookSet
    {
        string[] args;

        private StatusHooks(BaseWrapper wrapper, List<string> args)
            : base(wrapper)
        {
            this.args = args.ToArray();
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new StatusHooks(wrapper, args);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Status, HookLocation.InRepo, args),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Status, HookLocation.Normal, args));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Status, HookLocation.InRepo, args),
                () => this.Wrapper.FireAllHooks(HookType.Post_Status, HookLocation.Normal, args));
        }
    }
}
