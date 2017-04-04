using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class BranchArgs : IEnumerable<string>
    {
        public string TargetBranch;
        public bool Deleting;

        public BranchArgs()
        {
        }

        public BranchArgs(string[] args, int startingIndex = 0)
        {
            if (args.Length < 1 + startingIndex)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.TargetBranch = args[startingIndex];
            this.Deleting = args.Contains("-D");
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return TargetBranch;
            if (this.Deleting)
            {
                yield return "-D";
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
