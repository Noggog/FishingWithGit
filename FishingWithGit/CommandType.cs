using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public enum CommandType
    {
        checkout,
        rebase,
        reset,
        commit,
        commitmsg
    }

    public static class CommandTypeExt
    {
        public static bool TryParse(string str, out CommandType command)
        {
            if (Enum.TryParse(str, out command))
            {
                return true;
            }
            else if (str.Equals("commit-msg"))
            {
                command = CommandType.commitmsg;
                return true;
            }
            return false;
        }
    }
}
