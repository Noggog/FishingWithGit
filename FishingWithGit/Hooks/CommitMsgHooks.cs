using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitMsgHooks : HookSet
    {
        CommitMsgArgs args;

        private CommitMsgHooks(BaseWrapper wrapper, List<string> args, int commandIndex)
            : base(wrapper)
        {
            if (args.Count <= commandIndex + 1)
            {
                throw new ArgumentException("Cannot run checkout hooks, as args are invald.  No content was found after checkout command.");
            }

            this.args = new CommitMsgArgs(
                new string[]
                {
                    args[commandIndex + 1]
                });
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new CommitMsgHooks(wrapper, args, commandIndex);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Commit_Msg, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Commit_Msg, HookLocation.Normal, args.ToArray()));
        }

        public override async Task<int> PostCommand()
        {
            return 0;
        }
    }
}
