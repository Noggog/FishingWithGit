using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public abstract class HookSet
    {
        public BaseWrapper Wrapper { get; private set; }
        public abstract IGitHookArgs Args { get; }
        public abstract HookType PreType { get; }
        public abstract bool PreHookNatural { get; }
        public abstract HookType PostType { get; }
        public abstract bool PostHookNatural { get; }

        public HookSet(BaseWrapper wrapper)
        {
            this.Wrapper = wrapper;
        }

        public virtual Task<int> PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHooks(this.PreType, this.PreHookNatural, Args.ToArray()),
                () => this.Wrapper.FireMassHooks(this.PreType, Args.ToArray()));
        }

        public virtual Task<int> PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHooks(this.PostType, this.PostHookNatural, Args.ToArray()),
                () => this.Wrapper.FireMassHooks(this.PostType, Args.ToArray()));
        }
    }
}
