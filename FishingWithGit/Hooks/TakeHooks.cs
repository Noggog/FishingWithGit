﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class TakeHooks : HookSet
    {
        string[] newArgs;

        private TakeHooks(BaseWrapper wrapper, string[] args, int commandIndex)
            : base(wrapper)
        {
            var argsList = args.ToList();
            commandIndex = argsList.IndexOf("--", commandIndex);
            if (commandIndex == -1)
            {
                throw new ArgumentException("Could not run take hooks.  Args invalid and missing '--'.");
            }
            newArgs = new string[args.Length - commandIndex];
            Array.Copy(args, commandIndex, newArgs, 0, newArgs.Length);
        }

        public static HookSet Factory(BaseWrapper wrapper, string[] args, int commandIndex)
        {
            return new TakeHooks(wrapper, args, commandIndex);
        }

        public override int PreCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Pre_Take, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireHook(HookType.Pre_Take, HookLocation.Normal, newArgs));
        }

        public override int PostCommand()
        {
            return CommonFunctions.RunCommands(
                () => this.Wrapper.FireHook(HookType.Post_Take, HookLocation.InRepo, newArgs),
                () => this.Wrapper.FireHook(HookType.Post_Take, HookLocation.Normal, newArgs));
        }
    }
}