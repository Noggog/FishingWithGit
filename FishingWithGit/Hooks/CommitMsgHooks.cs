using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitMsgHooks : HookSet
    {
        string[] newArgs;

        private CommitMsgHooks(BaseWrapper wrapper, List<string> args, int commandIndex)
            : base(wrapper)
        {
            if (args.Count <= commandIndex + 1)
            {
                throw new ArgumentException("Cannot run checkout hooks, as args are invald.  No content was found after checkout command.");
            }

            newArgs = new string[]
            {
                args[commandIndex + 1]
            };
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            return new CommitMsgHooks(wrapper, args, commandIndex);
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Commit_Msg, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Commit_Msg, HookLocation.Normal, newArgs));
        }

        public override int PostCommand()
        {
            return 0;
        }
    }
}
