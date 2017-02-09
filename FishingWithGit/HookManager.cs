using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    class HookManager
    {
        public HookPair GetHook(string[] args)
        {
            if (args.Length == 0) return null;
            switch (args[0].ToLower())
            {
                case "checkout":
                    return CheckoutHooks(args);
                case "rebase":
                    return RebaseHooks(args);
                default:
                    return null;
            }
        }

        public HookPair CheckoutHooks(string[] args)
        {
            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireInRepoHook(CommandType.Pre_Checkout.HookName());
                    FireNormalHook(CommandType.Pre_Checkout.HookName());
                },
                PostCommand = () =>
                {
                    FireInRepoHook(CommandType.Post_Checkout.HookName());
                    // Normal already exists.
                }
            };
        }

        public HookPair RebaseHooks(string[] args)
        {
            return new HookPair()
            {
                PreCommand = () =>
                {
                    FireInRepoHook(CommandType.Pre_Rebase.HookName());
                    // Normal already exists.
                },
                PostCommand = () =>
                {
                    FireInRepoHook(CommandType.Post_Rebase.HookName());
                    FireNormalHook(CommandType.Post_Rebase.HookName());
                }
            };
        }

        public void FireNormalHook(string hookName)
        {

        }

        public void FireInRepoHook(string hookName)
        {

        }
    }
}
