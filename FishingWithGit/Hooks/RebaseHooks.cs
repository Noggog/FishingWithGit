using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class RebaseHooks : HookSet
    {
        private RebaseHooks(BaseWrapper wrapper) 
            : base(wrapper)
        {
        }

        public static HookSet Factory(BaseWrapper wrapper, string[] args, int commandIndex)
        {
            return new RebaseHooks(wrapper);
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Pre_Rebase, HookLocation.InRepo),
                () => this.Wrapper.FireExeHooks(HookType.Pre_Rebase, HookLocation.Normal));
        }

        public override int PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Post_Rebase, HookLocation.InRepo),
                () => this.Wrapper.FireHook(HookType.Post_Rebase, HookLocation.Normal));
        }
    }
}
