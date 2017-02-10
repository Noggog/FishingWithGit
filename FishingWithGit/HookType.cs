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
        Prepare_Commit_Msg,
        Commit_Msg,
        Pre_Rebase,
        Post_Rebase,
        Pre_Merge,
        Post_Merge,
        Pre_Push,
        Post_Push,
        Pre_Reset,
        Post_Reset
    }

    public static class HookTypeExt
    {
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
                case HookType.Prepare_Commit_Msg:
                case HookType.Commit_Msg:
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
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
