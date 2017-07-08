using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class PushArgs : IGitHookArgs
    {
        string[] args;
        public bool Silent => false;

        public string Remote;
        public List<Tuple<string, string>> RefSpecs;

        public PushArgs(string[] args)
        {
            this.args = args;
            this.RefSpecs = GetRefSpecs(args, out var remoteIndex);
            if (remoteIndex >= 0)
            {
                this.Remote = args[remoteIndex];
            }
        }

        private List<Tuple<string, string>> GetRefSpecs(string[] args, out int remoteIndex)
        {
            var ret = new List<Tuple<string, string>>();
            remoteIndex = args.Length - 1;
            for (; remoteIndex >= 0; remoteIndex--)
            {
                var arg = args[remoteIndex];
                var split = arg.Split(':');
                if (split.Length != 2) break;
                ret.Add(new Tuple<string, string>(
                    split[0],
                    split[1]));
            }
            if (ret.Count == 0)
            {
                remoteIndex = -1;
            }
            return ret;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var str in args)
            {
                yield return str;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
