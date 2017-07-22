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
        public override IGitHookArgs Args => args;
        public override HookType PreType => HookType.Commit_Msg;
        public override bool PreHookNatural => true;
        public override HookType PostType => throw new NotImplementedException();
        public override bool PostHookNatural => throw new NotImplementedException();

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

        public override async Task<int> PostCommand()
        {
            return 0;
        }
    }
}
