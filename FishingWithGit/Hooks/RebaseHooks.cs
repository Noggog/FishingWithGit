using FishingWithGit.Common;
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

        IGitHookArgs preArgs;
        IGitHookArgs postArgs;
        Type type;
        public override IGitHookArgs Args => preArgs;

        private RebaseHooks(BaseWrapper wrapper, Type type, IGitHookArgs preArgs, IGitHookArgs postArgs)
            : base(wrapper)
        {
            this.preArgs = preArgs;
            this.postArgs = postArgs;
            this.type = type;
        }

        public static HookSet Factory(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
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
                    return FactoryNormal(wrapper, repoDir, args, commandIndex);
            }
        }

        private static HookSet FactoryNormal(BaseWrapper wrapper, DirectoryInfo repoDir, List<string> args, int commandIndex)
        {
            using (var repo = new Repository(repoDir.FullName))
            {
                var targetBranch = repo.Branches[args[commandIndex + 1]];
                if (targetBranch == null)
                {
                    throw new ArgumentException($"Target branch did not exist {args[commandIndex + 1]}");
                }
                var rebaseArgs = new RebaseArgs(
                    new string[]
                    {
                        args[commandIndex + 1]
                    });
                var rebasePostArgs = new RebaseInProgressArgs(
                    new string[]
                    {
                        repo.Head.Tip.Sha,
                        targetBranch.Tip.Sha
                    });
                return new RebaseHooks(
                    wrapper,
                    Type.Normal,
                    rebaseArgs,
                    rebasePostArgs);
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
                rebaseArgs,
                rebaseArgs);
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
            if (line.Length != Constants.SHA_LENGTH)
            {
                throw new ArgumentException($"Unexpected length of line in {file.FullName}: {line.Length}, expected {Constants.SHA_LENGTH} for a Sha.");
            }

            return line;
        }

        public override Task<int> PreCommand()
        {
            switch (this.type)
            {
                case Type.Abort:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Abort, HookLocation.InRepo, preArgs.ToArray()),
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Abort, HookLocation.Normal, preArgs.ToArray()));
                case Type.Continue:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Continue, HookLocation.InRepo, preArgs.ToArray()),
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase_Continue, HookLocation.Normal, preArgs.ToArray()));
                case Type.Normal:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Pre_Rebase, HookLocation.InRepo, preArgs.ToArray()),
                        () => this.Wrapper.FireUnnaturalHooks(HookType.Pre_Rebase, HookLocation.Normal, preArgs.ToArray()));
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
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Abort, HookLocation.InRepo, postArgs.ToArray()),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Abort, HookLocation.Normal, postArgs.ToArray()));
                case Type.Continue:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Continue, HookLocation.InRepo, postArgs.ToArray()),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase_Continue, HookLocation.Normal, postArgs.ToArray()));
                case Type.Normal:
                    return CommonFunctions.RunCommands(
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase, HookLocation.InRepo, postArgs.ToArray()),
                        () => this.Wrapper.FireAllHooks(HookType.Post_Rebase, HookLocation.Normal, postArgs.ToArray()));
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
