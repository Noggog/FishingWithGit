using FishingWithGit.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CherryPickArgs : IGitHookArgs
    {
        public string PickedSha;
        public bool Silent => false;

        public CherryPickArgs()
        {
        }

        public CherryPickArgs(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException($"Checkout args were shorter than expected: {args.Length}");
            }

            this.PickedSha = args[0];

            if (PickedSha.Length != Constants.SHA_LENGTH)
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
