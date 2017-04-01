using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public static class HookTypeCommandExt
    {
        private static Lazy<HashSet<string>> commandStrings = new Lazy<HashSet<string>>(
            () =>
            {
                var ret = new HashSet<string>();
                foreach (HookType type in Enum.GetValues(typeof(HookType)))
                {
                    try
                    {
                        ret.Add(type.CommandString());
                    }
                    catch
                    {
                    }
                }
                return ret;
            });

        public static bool IsCommandString(string str)
        {
            return commandStrings.Value.Contains(str);
        }
        
        public static CommandType AssociatedCommand(this HookType type)
        {
            switch (type)
            {
                case HookType.Pre_Checkout:
                case HookType.Post_Checkout:
                case HookType.Pre_Take:
                case HookType.Post_Take:
                    return CommandType.checkout;
                case HookType.Pre_ApplyPatch:
                case HookType.Post_ApplyPatch:
                    throw new NotImplementedException();
                case HookType.Pre_Commit:
                case HookType.Post_Commit:
                    return CommandType.commit;
                case HookType.Prepare_Commit_Msg:
                    throw new NotImplementedException();
                case HookType.Commit_Msg:
                    return CommandType.commitmsg;
                case HookType.Pre_Rebase:
                case HookType.Post_Rebase:
                case HookType.Pre_Rebase_Abort:
                case HookType.Post_Rebase_Abort:
                case HookType.Pre_Rebase_Continue:
                case HookType.Post_Rebase_Continue:
                    return CommandType.rebase;
                case HookType.Pre_Merge:
                case HookType.Post_Merge:
                    return CommandType.merge;
                case HookType.Pre_Push:
                case HookType.Post_Push:
                    return CommandType.push;
                case HookType.Pre_Reset:
                case HookType.Post_Reset:
                    return CommandType.reset;
                case HookType.Pre_Status:
                case HookType.Post_Status:
                    return CommandType.status;
                case HookType.Pre_CherryPick:
                case HookType.Post_CherryPick:
                    return CommandType.cherry;
                case HookType.Pre_Pull:
                case HookType.Post_Pull:
                    return CommandType.pull;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
