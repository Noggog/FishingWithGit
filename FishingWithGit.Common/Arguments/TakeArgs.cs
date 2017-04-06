using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TakeArgs : IGitHookArgs
    {
        public string TargetSha;
        public List<string> Items;
        public bool Silent => false;

        public TakeArgs()
        {
        }

        public TakeArgs(string[] args)
        {
            if (args.Length < 1
                || args[0].Length != 40)
            {
                throw new ArgumentException("No target sha specified.");
            }
            this.TargetSha = args[0];
            this.Items = new List<string>();
            for (int i = 1; i < args.Length; i++)
            {
                this.Items.Add(args[i]);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return TargetSha;
            if (Items == null) yield break;
            foreach (var item in this.Items)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
