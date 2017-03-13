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
        public string OriginalSha;
        public string TargetSha;

        public RebaseInProgressArgs(string[] args, int startingIndex = 0)
        {
            if (args.Length < 2 + startingIndex)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.OriginalSha = args[startingIndex];
            this.TargetSha = args[startingIndex + 1];
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return OriginalSha;
            yield return TargetSha;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
