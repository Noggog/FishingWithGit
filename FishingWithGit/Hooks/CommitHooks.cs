using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitHooks : HookSet
    {
        bool amend;

        private CommitHooks(BaseWrapper wrapper, List<string> args)
            : base(wrapper)
        {
            this.amend = args.Contains("--amend");
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new CommitHooks(wrapper, args);
        }

        public override Task<int> PreCommand()
        {
            string[] args = amend ? new string[] { "--amend" } : new string[0];
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Commit, HookLocation.InRepo, args),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_Commit, HookLocation.Normal, args));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Commit, HookLocation.InRepo),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Post_Commit, HookLocation.Normal));
        }
    }
}
