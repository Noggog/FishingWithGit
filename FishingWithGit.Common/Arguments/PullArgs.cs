using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class PullArgs : IEnumerable<string>
    {
        public string AncestorSha;
        public string TargetSha;

        public PullArgs(string[] args, int startingIndex = 0)
        {
            if (args.Length < 2 + startingIndex)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.AncestorSha = args[startingIndex];
            this.TargetSha = args[startingIndex + 1];
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
