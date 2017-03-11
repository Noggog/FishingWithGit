using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public enum HookType
    {
        Pre_Checkout,
        Post_Checkout,
        Pre_ApplyPatch,
        Post_ApplyPatch,
        Pre_Commit,
        Post_Commit,
        Pre_CherryPick,
        Post_CherryPick,
        Prepare_Commit_Msg,
        Commit_Msg,
        Pre_Rebase,
        Post_Rebase,
        Pre_Rebase_Abort,
        Post_Rebase_Abort,
        Pre_Rebase_Continue,
        Post_Rebase_Continue,
        Pre_Merge,
        Post_Merge,
        Pre_Push,
        Post_Push,
        Pre_Reset,
        Post_Reset,
        Pre_Status,
        Post_Status,
        Pre_Take,
        Post_Take,
    }

    public static class HookTypeExt
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

        private static Lazy<HashSet<string>> hookStrings = new Lazy<HashSet<string>>(
            () =>
            {
                var ret = new HashSet<string>();
                foreach (HookType type in Enum.GetValues(typeof(HookType)))
                {
                    try
                    {
                        ret.Add(type.HookName());
                    }
                    catch
                    {
                    }
                }
                return ret;
            });

        public static bool IsHookName(string name)
        {
            return hookStrings.Value.Contains(name);
        }

        public static bool Stock(this HookType type)
        {
            switch (type)
            {
                case HookType.Post_Checkout:
                case HookType.Pre_ApplyPatch:
                case HookType.Post_ApplyPatch:
                case HookType.Pre_Commit:
                case HookType.Post_Commit:
                case HookType.Prepare_Commit_Msg:
                case HookType.Commit_Msg:
                case HookType.Pre_Rebase:
                case HookType.Post_Merge:
                case HookType.Pre_Push:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Custom(this HookType type)
        {
            return type.Stock();
        }

        public static string CommandString(this HookType type)
        {
            switch (type)
            {
                case HookType.Pre_Checkout:
                case HookType.Post_Checkout:
                    return "checkout";
                case HookType.Pre_Commit:
                case HookType.Post_Commit:
                    return "commit";
                case HookType.Pre_Rebase:
                case HookType.Post_Rebase:
                case HookType.Pre_Rebase_Abort:
                case HookType.Post_Rebase_Abort:
                case HookType.Pre_Rebase_Continue:
                case HookType.Post_Rebase_Continue:
                    return "rebase";
                case HookType.Pre_Merge:
                case HookType.Post_Merge:
                    return "merge";
                case HookType.Pre_Push:
                case HookType.Post_Push:
                    return "push";
                case HookType.Pre_Reset:
                case HookType.Post_Reset:
                    return "reset";
                case HookType.Pre_ApplyPatch:
                case HookType.Post_ApplyPatch:
                    return "apply";
                case HookType.Commit_Msg:
                    return "commit-msg";
                case HookType.Pre_Status:
                case HookType.Post_Status:
                    return "status";
                case HookType.Pre_Take:
                case HookType.Post_Take:
                    return "take";
                case HookType.Pre_CherryPick:
                case HookType.Post_CherryPick:
                    return "cherry-pick";
                case HookType.Prepare_Commit_Msg:
                default:
                    throw new NotImplementedException();
            }
        }

        public static string HookName(this HookType type)
        {
            switch (type)
            {
                case HookType.Pre_Checkout:
                    return "pre-checkout";
                case HookType.Post_Checkout:
                    return "post-checkout";
                case HookType.Pre_ApplyPatch:
                    return "pre-applypatch";
                case HookType.Post_ApplyPatch:
                    return "post-applypatch";
                case HookType.Pre_Commit:
                    return "pre-commit";
                case HookType.Post_Commit:
                    return "post-commit";
                case HookType.Prepare_Commit_Msg:
                    return "prepare-commit-msg";
                case HookType.Commit_Msg:
                    return "commit-msg";
                case HookType.Pre_Rebase:
                    return "pre-rebase";
                case HookType.Post_Rebase:
                    return "post-rebase";
                case HookType.Pre_Rebase_Abort:
                    return "pre-rebase-abort";
                case HookType.Post_Rebase_Abort:
                    return "post-rebase-abort";
                case HookType.Pre_Rebase_Continue:
                    return "pre-rebase-continue";
                case HookType.Post_Rebase_Continue:
                    return "post-rebase-continue";
                case HookType.Pre_Merge:
                    return "pre-merge";
                case HookType.Post_Merge:
                    return "post-merge";
                case HookType.Pre_Push:
                    return "pre-push";
                case HookType.Post_Push:
                    return "post-push";
                case HookType.Pre_Reset:
                    return "pre-reset";
                case HookType.Post_Reset:
                    return "post-reset";
                case HookType.Pre_Status:
                    return "pre-status";
                case HookType.Post_Status:
                    return "post-status";
                case HookType.Pre_Take:
                    return "pre-take";
                case HookType.Post_Take:
                    return "post-take";
                case HookType.Pre_CherryPick:
                    return "pre-cherry-pick";
                case HookType.Post_CherryPick:
                    return "post-cherry-pick";
                default:
                    throw new NotImplementedException();
            }
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
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
