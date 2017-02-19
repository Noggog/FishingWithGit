using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public static class CommonFunctions
    {
        public static int RunCommands(params Func<int>[] funcs)
        {
            foreach (var func in funcs)
            {
                var code = func();
                if (code != 0)
                {
                    return code;
                }
            }
            return 0;
        }

        public static int Skip(string[] args, int startIndex, params string[] commands)
        {
            for (; startIndex < args.Length; startIndex++)
            {
                if (commands.Contains(args[startIndex])) continue;
                return startIndex;
            }
            return startIndex;
        }
    }
}
