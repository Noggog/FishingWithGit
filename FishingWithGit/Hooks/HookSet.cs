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
        public abstract int PreCommand();
        public abstract int PostCommand();

        public HookSet(BaseWrapper wrapper)
        {
            this.Wrapper = wrapper;
        }
    }
}
