using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class HookPair
    {
        public Func<int> PreCommand;
        public Func<int> PostCommand;
    }
}
