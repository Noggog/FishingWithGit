using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TagArgs : IGitHookArgs
    {
        string[] args;
        public bool Silent => true;

        public TagArgs(string[] args)
        {
            this.args = args;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var str in args)
            {
                yield return str;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
