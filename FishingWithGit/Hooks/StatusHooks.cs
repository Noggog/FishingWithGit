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

        private StatusHooks(BaseWrapper wrapper, string[] args)
            : base(wrapper)
        {
            this.args = args;
        }

        public static HookSet Factory(BaseWrapper wrapper, string[] args, int commandIndex)
        {
            return new StatusHooks(wrapper, args);
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Pre_Status, HookLocation.InRepo, args),
                () => this.Wrapper.FireHook(HookType.Pre_Status, HookLocation.Normal, args));
        }

        public override int PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Post_Status, HookLocation.InRepo, args),
                () => this.Wrapper.FireHook(HookType.Post_Status, HookLocation.Normal, args));
        }
    }
}
