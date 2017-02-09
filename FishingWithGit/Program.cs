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
        static void Main(string[] args)
        {
            BaseWrapper wrapper = new BaseWrapper();
            wrapper.Wrap(args);
        }
    }
}
