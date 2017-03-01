using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class ResetProcessor : ArgProcessor
    {
        public override void Process(List<string> args)
        {
            if (args.Contains("--mixed")
                || args.Contains("--hard")
                || args.Contains("--soft")) return;
            ProcessAfterSplitterFileList(args);
        }
    }
}
