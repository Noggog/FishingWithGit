using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class Program
    {
        static int Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            BaseWrapper wrapper = new BaseWrapper(args);
            return wrapper.Wrap(sw).Result;
        }
    }
}
