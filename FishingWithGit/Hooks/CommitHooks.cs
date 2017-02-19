using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitHooks : HookSet
    {
        private CommitHooks(BaseWrapper wrapper)
            : base(wrapper)
        {
        }

        public static HookSet Factory(BaseWrapper wrapper, string[] args, int commandIndex)
        {
            return new CommitHooks(wrapper);
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Pre_Commit, HookLocation.InRepo),
                () => this.Wrapper.FireExeHooks(HookType.Pre_Commit, HookLocation.Normal));
        }

        public override int PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Post_Commit, HookLocation.InRepo),
                () => this.Wrapper.FireExeHooks(HookType.Post_Commit, HookLocation.Normal));
        }
    }
}
