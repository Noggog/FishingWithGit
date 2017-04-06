using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class BranchArgs : IGitHookArgs
    {
        public string TargetBranch;
        public bool Deleting;
        public bool Silent
        {
            get
            {
                return !Deleting && string.IsNullOrWhiteSpace(TargetBranch);
            }
        }

        public BranchArgs()
        {
        }

        public BranchArgs(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            this.TargetBranch = args[0];
            this.Deleting = args.Contains("-D");
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (!string.IsNullOrWhiteSpace(this.TargetBranch))
            {
                yield return TargetBranch;
            }
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
