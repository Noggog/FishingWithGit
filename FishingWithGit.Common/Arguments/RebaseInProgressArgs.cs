using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class RebaseInProgressArgs : IEnumerable<string>
    {
        public string OriginalTipSha;
        public string LandingSha;

        public RebaseInProgressArgs()
        {
        }

        public RebaseInProgressArgs(string[] args, int startingIndex = 0)
        {
            if (args.Length < 2 + startingIndex)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.OriginalTipSha = args[startingIndex];
            this.LandingSha = args[startingIndex + 1];
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return OriginalTipSha;
            yield return LandingSha;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
