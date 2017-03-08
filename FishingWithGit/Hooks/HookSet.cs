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
        public abstract Task<int> PreCommand();
        public abstract Task<int> PostCommand();

        public HookSet(BaseWrapper wrapper)
        {
            this.Wrapper = wrapper;
        }
    }
}
