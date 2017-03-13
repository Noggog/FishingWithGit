using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class MergeArgs : IEnumerable<string>
    {
        public string TargetBranch;

        public MergeArgs(string[] args, int startIndex = 0)
        {
            if (args.Length < 1 + startIndex)
            {
                throw new ArgumentException("Target branch argument was not provided.");
            }

            this.TargetBranch = args[startIndex];
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return TargetBranch;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
