﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class AddProcessor : ArgProcessor
    {
        public override void Process(List<string> args)
        {
            ProcessAfterSplitterFileList(args);
        }
    }
}