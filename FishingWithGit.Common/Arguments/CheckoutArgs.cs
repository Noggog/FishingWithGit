using FishingWithGit.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CheckoutArgs : IGitHookArgs
    {
        public string BranchName;
        public string CurrentSha;
        public string TargetSha;
        public bool Silent => false;

        public CheckoutArgs()
        {
        }

        public CheckoutArgs(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException($"Checkout args were shorter than expected: {args.Length}");
            }

            this.CurrentSha = args[0];
            this.TargetSha = args[1];

            if (CurrentSha.Length != Constants.SHA_LENGTH)
            {
                throw new ArgumentException($"Checkout args for current sha was shorter than expected: {this.CurrentSha.Length} {this.CurrentSha}");
            }
            if (TargetSha.Length != Constants.SHA_LENGTH)
            {
                throw new ArgumentException($"Checkout args for target sha was shorter than expected: {this.TargetSha.Length} {this.TargetSha}");
            }

            if (args.Length >= 3)
            {
                this.BranchName = args[2];
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return CurrentSha;
            yield return TargetSha;
            if (!string.IsNullOrWhiteSpace(this.BranchName))
            {
                yield return BranchName;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
