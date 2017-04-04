using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TakeArgs : IEnumerable<string>
    {
        public List<string> Items;

        public TakeArgs()
        {
        }

        public TakeArgs(string[] args)
        {
            this.Items = args.ToList();
        }

        public IEnumerator<string> GetEnumerator()
        {
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
