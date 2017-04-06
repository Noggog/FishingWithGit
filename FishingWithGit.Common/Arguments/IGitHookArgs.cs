using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public interface IGitHookArgs : IEnumerable<string>
    {
        bool Silent { get; }
    }
}
