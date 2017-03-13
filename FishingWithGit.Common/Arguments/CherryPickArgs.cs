using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CherryPickArgs : IEnumerable<string>
    {
        public string PickedSha;

        public CherryPickArgs(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException($"Checkout args were shorter than expected: {args.Length}");
            }

            this.PickedSha = args[0];

            if (PickedSha.Length != 40)
            {
                throw new ArgumentException($"Checkout args for picked sha was shorter than expected: {this.PickedSha.Length} {this.PickedSha}");
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return PickedSha;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
