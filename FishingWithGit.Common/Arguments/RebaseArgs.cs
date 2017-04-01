using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class RebaseArgs : IEnumerable<string>
    {
        public string TargetBranch;

        public RebaseArgs()
        {
        }

        public RebaseArgs(string[] args, int startingIndex = 0)
        {
            if (args.Length < 1 + startingIndex)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.TargetBranch = args[startingIndex];
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
