using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitArgs : IEnumerable<string>
    {
        public bool Amending;
        public string CommitMessageFile;

        public CommitArgs(string[] args)
        {
            this.Amending = args.Contains("--amend");
        }

        public IEnumerator<string> GetEnumerator()
        {
            if (Amending)
            {
                yield return "--amend";
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
