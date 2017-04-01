using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CommitMsgArgs : IEnumerable<string>
    {
        public string CommitMessageFilePath;

        public CommitMsgArgs()
        {
        }

        public CommitMsgArgs(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Target file argument was not provided.");
            }

            this.CommitMessageFilePath = args[0];
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return CommitMessageFilePath;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
