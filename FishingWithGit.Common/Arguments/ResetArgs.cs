using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class ResetArgs : IEnumerable<string>
    {
        public string Branch;
        public string StartingSha;
        public string TargetSha;
        public ResetType Type;

        public ResetArgs(string[] args)
        {
            if (args.Length < 4)
            {
                throw new ArgumentException("Unexpected amount of args.");
            }

            this.Branch = args[0];
            this.StartingSha = args[1];
            this.TargetSha = args[2];

            if (!Enum.TryParse(args[3], out this.Type))
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return Branch;
            yield return StartingSha;
            yield return TargetSha;
            yield return Type.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
