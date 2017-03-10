using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class CherryPickHook : HookSet
    {
        string sha;

        private CherryPickHook(BaseWrapper wrapper, string sha)
            : base(wrapper)
        {
            this.sha = sha;
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count <= commandIndex
                || args[commandIndex + 1].Length != 40)
            {
                throw new ArgumentException("Cherry pick did not have a target sha.");
            }
            return new CherryPickHook(wrapper, args[commandIndex + 1]);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_CherryPick, HookLocation.InRepo, sha),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_CherryPick, HookLocation.Normal, sha));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_CherryPick, HookLocation.InRepo, sha),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Post_CherryPick, HookLocation.Normal, sha));
        }
    }
}

