using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public enum CommandType
    {
        unknown,
        checkout,
        rebase,
        reset,
        commit,
        status,
        merge,
        push,
        commitmsg,
        clone,
        add,
        cherry,
        pull,
        branch,
        tag
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
            else if (str.Equals("cherry-pick"))
            {
                command = CommandType.cherry;
                return true;
            }
            return false;
        }
    }
}
