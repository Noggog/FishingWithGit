﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class RebaseArgs : IGitHookArgs
    {
        public string Target;
        public bool Skip;
        public bool Silent => false;

        public RebaseArgs()
        {
        }

        public RebaseArgs(string[] args)
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }

            this.Skip = "--skip".Equals(args[0]);
            if (!Skip)
            {
                this.Target = args[0];
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            yield return Target;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
