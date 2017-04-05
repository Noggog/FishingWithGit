using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests
{
    public class BranchTests
    {
        public const string BRANCH = "BranchName";
        public const string DELETE_TAG = "-D";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                BRANCH
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new BranchArgs(GetBasicOutgoingArgs());
            Assert.Equal(BRANCH, args.TargetBranch);
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new BranchArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(1, list.Count);
            Assert.Equal(BRANCH, list[0]);
        }

        public static string[] GetDeleteOutgoingArgs()
        {
            return new string[]
            {
                BRANCH,
                DELETE_TAG
            };
        }

        [Fact]
        public static void DeleteArgs()
        {
            var args = new BranchArgs(GetDeleteOutgoingArgs());
            Assert.Equal(BRANCH, args.TargetBranch);
            Assert.Equal(true, args.Deleting);
        }

        [Fact]
        public static void DeleteArgs_Enumeration()
        {
            var args = new BranchArgs(GetDeleteOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(BRANCH, list[0]);
            Assert.Equal(DELETE_TAG, list[1]);
        }

        public static string[] GetSourceTreeInboundArgs()
        {
            return new string[]
            {
                "--no-pager",
                "-c",
                "color.branch=false",
                "-c",
                "color.diff=false",
                "-c",
                "color.status=false",
                "-c",
                "diff.mnemonicprefix=false",
                "-c",
                "core.quotepath=false",
                "branch",
                "ssss"
            };
        }

        public const int COMMAND_INDEX = 11;

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "branch",
                "ssss"
            };
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.branch, args);
            Assert.Equal(GetProcessedArgs(), args);
        }

        [Fact]
        public static void HookSetCtor()
        {
            var hook = BranchHooks.Factory(
                null,
                GetSourceTreeInboundArgs().ToList(),
                COMMAND_INDEX);
        }

        public static string[] GetBranchListInboundArgs()
        {
            return new string[]
            {
                "--no-pager",
                "-c",
                "color.branch=false",
                "-c",
                "color.diff=false",
                "-c",
                "color.status=false",
                "-c",
                "diff.mnemonicprefix=false",
                "-c",
                "core.quotepath=false",
                "branch"
            };
        }

        [Fact]
        public static void HookSetCtor_BranchListArgs()
        {
            var hook = BranchHooks.Factory(
                null,
                GetBranchListInboundArgs().ToList(),
                COMMAND_INDEX);
        }

        public static string[] GetDeleteInboundArgs()
        {
            return new string[]
            {
                "--no-pager",
                "-c",
                "color.branch=false",
                "-c",
                "color.diff=false",
                "-c",
                "color.status=false",
                "-c",
                "diff.mnemonicprefix=false",
                "-c",
                "core.quotepath=false",
                "branch",
                "ssss",
                "-D"
            };
        }

        [Fact]
        public static void HookSetCtor_Delete()
        {
            var hook = BranchHooks.Factory(
                null,
                GetDeleteInboundArgs().ToList(),
                COMMAND_INDEX);
        }
    }
}