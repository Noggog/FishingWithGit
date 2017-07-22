using FishingWithGit.Common;
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
        public override HookType PreType => HookType.Pre_CherryPick;
        public override bool PreHookNatural => false;
        public override HookType PostType => HookType.Post_CherryPick;
        public override bool PostHookNatural => false;

        private CherryPickHook(BaseWrapper wrapper, CherryPickArgs args)
            : base(wrapper)
        {
            this.args = args;
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count <= commandIndex
                || args[commandIndex + 1].Length != Constants.SHA_LENGTH)
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
    }
}

