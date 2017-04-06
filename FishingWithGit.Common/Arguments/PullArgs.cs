using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class PullArgs : IGitHookArgs
    {
        public string AncestorSha;
        public string TargetSha;
        public bool Silent => false;

        public PullArgs()
        {
        }

        public PullArgs(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.AncestorSha = args[0];
            this.TargetSha = args[1];
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return AncestorSha;
            yield return TargetSha;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
