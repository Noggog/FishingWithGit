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

        public TakeArgs(string[] args)
        {
            this.Items = args.ToList();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
