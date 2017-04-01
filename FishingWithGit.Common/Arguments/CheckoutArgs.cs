using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CheckoutArgs : IEnumerable<string>
    {
        public string CurrentSha;
        public string TargetSha;

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

            if (CurrentSha.Length != 40)
            {
                throw new ArgumentException($"Checkout args for current sha was shorter than expected: {this.CurrentSha.Length} {this.CurrentSha}");
            }
            if (TargetSha.Length != 40)
            {
                throw new ArgumentException($"Checkout args for target sha was shorter than expected: {this.TargetSha.Length} {this.TargetSha}");
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return CurrentSha;
            yield return TargetSha;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
