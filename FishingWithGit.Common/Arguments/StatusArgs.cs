using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class StatusArgs : IGitHookArgs
    {
        public bool Silent => true;

        public StatusArgs()
        {
        }

        public StatusArgs(string[] args)
        {
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
