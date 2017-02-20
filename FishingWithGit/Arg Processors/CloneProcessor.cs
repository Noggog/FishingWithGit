using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class CloneProcessor : ArgProcessor
    {
        public override void Process(List<string> args)
        {
            if (!Properties.Settings.Default.RemoveTemplateFromClone) return;
            var tmp = args.ToList();
            args.Clear();
            args.AddRange(
                tmp.Where((arg) => !arg.StartsWith("--template")));
        }
    }
}
