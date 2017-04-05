using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public class RebaseHooks : HookSet
    {
        enum Type { Normal, Abort, Continue }

        string[] preArgs;
        string[] postArgs;
        Type type;

        private RebaseHooks(BaseWrapper wrapper, Type type, string[] preArgs, string[] postArgs)
            : base(wrapper)
        {
            this.preArgs = preArgs;
            this.postArgs = postArgs;
            this.type = type;
        }

        public static HookSet Factory(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            if (args.Count <= commandIndex + 1)
            {
                throw new ArgumentException("An argument was expected but did not exist");
            }
            
            switch (args[commandIndex + 1])
            {
                case "--abort":
                    return FactoryInProgress(wrapper, args, commandIndex, Type.Abort);
                case "--continue":
                    return FactoryInProgress(wrapper, args, commandIndex, Type.Continue);
                default:
                    return FactoryNormal(wrapper, args, commandIndex);
            }
        }

        private static HookSet FactoryNormal(BaseWrapper wrapper, List<string> args, int commandIndex)
        {
            using (var repo = new Repository(Directory.GetCurrentDirectory()))
            {
                var targetBranch = repo.Branches[args[commandIndex + 1]];
                if (targetBranch == null)
                {
                    throw new ArgumentException($"Target branch did not exist {args[commandIndex + 1]}");
                }
                var rebaseArgs = new RebaseArgs(
                    args.ToArray(),
                    commandIndex + 1);
                var rebasePostArgs = new RebaseInProgressArgs(
                    new string[]
                    {
                        repo.Head.Tip.Sha,
                        targetBranch.Tip.Sha
                    });
                return new RebaseHooks(
                    wrapper,
                    Type.Normal,
                    rebaseArgs.ToArray(),
                    rebasePostArgs.ToArray());
            }
        }

        private static HookSet FactoryInProgress(BaseWrapper wrapper, List<string> args, int commandIndex, Type type)
        {
            var rebaseArgs = new RebaseInProgressArgs(
                new string[]
                {
                    GetShaFromFile("orig-head"),
                    GetShaFromFile("abort-safety")
                });
            return new RebaseHooks(
                wrapper,
                type,
                rebaseArgs.ToArray(),
                rebaseArgs.ToArray());
        }

        private static string GetShaFromFile(string fileName)
        {
            FileInfo file = new FileInfo($".git/rebase-apply/{fileName}");
            if (!file.Exists)
            {
                throw new ArgumentException($"No file found at {file.FullName}");
            }

            var lines = File.ReadAllLines(file.FullName);
            if (lines.Length != 1)
            {
                throw new ArgumentException($"Unexpected number of lines found in {file.FullName}: {lines.Length}");
            }

            var line = lines[0];
            if (line.Length != 40)
            {
                throw new ArgumentException($"Unexpected length of line in {file.FullName}: {line.Length}, expected 40 for a Sha.");
            }

            return line;
        }

        public override Task<int> PreCommand()
        {
            switch (this.type)
            {
                case Type.Abort:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Abort, HookLocation.InRepo, preArgs),
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Abort, HookLocation.Normal, preArgs));
                case Type.Continue:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Continue, HookLocation.InRepo, preArgs),
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Continue, HookLocation.Normal, preArgs));
                case Type.Normal:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase, HookLocation.InRepo, preArgs),
                        () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_Rebase, HookLocation.Normal, preArgs));
                default:
                    throw new NotImplementedException();
            }
        }

        public override Task<int> PostCommand()
        {
            switch (this.type)
            {
                case Type.Abort:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Abort, HookLocation.InRepo, postArgs),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Abort, HookLocation.Normal, postArgs));
                case Type.Continue:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Continue, HookLocation.InRepo, postArgs),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Continue, HookLocation.Normal, postArgs));
                case Type.Normal:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase, HookLocation.InRepo, postArgs),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase, HookLocation.Normal, postArgs));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
