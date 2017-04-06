using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class CherryPickHook : HookSet
    {
        CherryPickArgs args;
        public override IGitHookArgs Args => args;

        private CherryPickHook(BaseWrapper wrapper, CherryPickArgs args)
            : base(wrapper)
        {
            this.args = args;
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count <= commandIndex
                || args[commandIndex + 1].Length != 40)
            {
                throw new ArgumentException("Cherry pick did not have a target sha.");
            }
            return new CherryPickHook(
                wrapper,
                new CherryPickArgs(
                    new string[]
                    {
                        args[commandIndex + 1]
                    }));
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_CherryPick, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_CherryPick, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_CherryPick, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireUnnaturalHooks(HookType.Post_CherryPick, HookLocation.Normal, args.ToArray()));
        }
    }
}

