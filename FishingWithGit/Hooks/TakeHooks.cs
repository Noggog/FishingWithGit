using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TakeHooks : HookSet
    {
        TakeArgs args;
        public override IGitHookArgs Args => args;

        private TakeHooks(BaseWrapper wrapper, string targetSha, List<string> args, int commandIndex)
            : base(wrapper)
        {
            var argsList = args.ToList();
            commandIndex = argsList.IndexOf("--", commandIndex);
            if (commandIndex == -1)
            {
                throw new ArgumentException("Could not run take hooks.  Args invalid and missing '--'.");
            }
            var newArgs = new string[args.Count - commandIndex];
            Array.Copy(args.ToArray(), commandIndex, newArgs, 0, newArgs.Length);
            this.args = new TakeArgs()
            {
                TargetSha = targetSha,
                Items = new List<string>(newArgs)
            };
        }

        public static HookSet Factory(BaseWrapper wrapper, string targetSha, List<string> args, int commandIndex)
        {
            return new TakeHooks(wrapper, targetSha, args, commandIndex);
        }

        public override Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Pre_Take, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Pre_Take, HookLocation.Normal, args.ToArray()));
        }

        public override Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireAllHooks(HookType.Post_Take, HookLocation.InRepo, args.ToArray()),
                () => this.Wrapper.FireAllHooks(HookType.Post_Take, HookLocation.Normal, args.ToArray()));
        }
    }
}
