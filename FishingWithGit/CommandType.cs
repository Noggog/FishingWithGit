using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public enum CommandType
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

    public static class CommandTypeExt
    {
        public static bool Stock(this CommandType type)
        {
            switch (type)
            {
                case CommandType.Post_Checkout:
                case CommandType.Pre_ApplyPatch:
                case CommandType.Post_ApplyPatch:
                case CommandType.Pre_Commit:
                case CommandType.Post_Commit:
                case CommandType.Prepare_Commit_Msg:
                case CommandType.Commit_Msg:
                case CommandType.Pre_Rebase:
                case CommandType.Post_Merge:
                case CommandType.Pre_Push:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Custom(this CommandType type)
        {
            return type.Stock();
        }

        public static string CommandString(this CommandType type)
        {
            switch (type)
            {
                case CommandType.Pre_Checkout:
                case CommandType.Post_Checkout:
                    return "checkout";
                case CommandType.Pre_Commit:
                case CommandType.Post_Commit:
                    return "commit";
                case CommandType.Pre_Rebase:
                case CommandType.Post_Rebase:
                    return "rebase";
                case CommandType.Pre_Merge:
                case CommandType.Post_Merge:
                    return "merge";
                case CommandType.Pre_Push:
                case CommandType.Post_Push:
                    return "push";
                case CommandType.Pre_Reset:
                case CommandType.Post_Reset:
                    return "reset";
                case CommandType.Pre_ApplyPatch:
                case CommandType.Post_ApplyPatch:
                    return "apply";
                case CommandType.Prepare_Commit_Msg:
                case CommandType.Commit_Msg:
                default:
                    throw new NotImplementedException();
            }
        }

        public static string HookName(this CommandType type)
        {
            switch (type)
            {
                case CommandType.Pre_Checkout:
                    return "pre-checkout";
                case CommandType.Post_Checkout:
                    return "post-checkout";
                case CommandType.Pre_ApplyPatch:
                    return "pre-applypatch";
                case CommandType.Post_ApplyPatch:
                    return "post-applypatch";
                case CommandType.Pre_Commit:
                    return "pre-commit";
                case CommandType.Post_Commit:
                    return "post-commit";
                case CommandType.Prepare_Commit_Msg:
                    return "prepare-commit-msg";
                case CommandType.Commit_Msg:
                    return "commit-msg";
                case CommandType.Pre_Rebase:
                    return "pre-rebase";
                case CommandType.Post_Rebase:
                    return "post-rebase";
                case CommandType.Pre_Merge:
                    return "pre-merge";
                case CommandType.Post_Merge:
                    return "post-merge";
                case CommandType.Pre_Push:
                    return "pre-push";
                case CommandType.Post_Push:
                    return "post-push";
                case CommandType.Pre_Reset:
                    return "pre-reset";
                case CommandType.Post_Reset:
                    return "post-reset";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
