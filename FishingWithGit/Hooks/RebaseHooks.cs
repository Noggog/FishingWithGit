using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class RebaseHooks : HookSet
    {
        string[] args;
        enum Type { Normal, Abort, Continue }
        Type type;

        private RebaseHooks(BaseWrapper wrapper, string[] args) 
            : base(wrapper)
        {
            this.args = args;
            switch (this.args[0])
            {
                case "--abort":
                    this.type = Type.Abort;
                    break;
                case "--continue":
                    this.type = Type.Continue;
                    break;
                default:
                    this.type = Type.Normal;
                    break;
            }
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count <= commandIndex + 1)
            {
                throw new ArgumentException("No target branch argument provided.");
            }

            var targetBranch = args[commandIndex + 1];
            var curBranch = args.Count > commandIndex + 2 ? args[commandIndex + 2] : null;
            var l = new List<string>();
            l.Add(targetBranch);
            if (curBranch != null)
            {
                l.Add(curBranch);
            }

            return new RebaseHooks(
                wrapper,
                l.ToArray());
        }

        public override Task<int> PreCommand()
        {
            switch (this.type)
            {
                case Type.Abort:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Abort, HookLocation.InRepo),
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Abort, HookLocation.Normal));
                case Type.Continue:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Continue, HookLocation.InRepo),
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Continue, HookLocation.Normal));
                case Type.Normal:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase, HookLocation.InRepo, args),
                        () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_Rebase, HookLocation.Normal, args));
                default:
                    throw new NotImplementedException();
            }
        }

        public override Task<int> PostCommand()
        {
            switch (this.type)
            {
                case Type.Abort:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Abort, HookLocation.InRepo),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Abort, HookLocation.Normal));
                case Type.Continue:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Continue, HookLocation.InRepo),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Continue, HookLocation.Normal));
                case Type.Normal:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase, HookLocation.InRepo, args),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase, HookLocation.Normal, args));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
