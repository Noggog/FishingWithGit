using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class RebaseTests
    {
        public const string TARGET_BRANCH = "master";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                TARGET_BRANCH
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new RebaseArgs(GetBasicOutgoingArgs());
            Assert.Equal(TARGET_BRANCH, args.TargetBranch);
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
                "rebase",
                "master",
            };
        }

        public const int COMMAND_INDEX = 11;

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "rebase",
                "master",
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new RebaseArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(1, list.Count);
            Assert.Equal(TARGET_BRANCH, list[0]);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.rebase, args);
            Assert.Equal(GetProcessedArgs(), args);
        }

        [Fact]
        public static void HookSetCtor()
        {
            using (var checkout = Repository_Tools.GetTypicalRepo())
            {
                var hook = RebaseHooks.Factory(
                    null,
                    checkout.Dir,
                    GetSourceTreeInboundArgs().ToList(),
                    COMMAND_INDEX);
            }
        }

        public const string CURRENT_SHA = "92d1ea36962347d2f072d3d67a5b4842a0d9cf74";
        public const string TARGET_SHA = "c5ce43478954719c5577bbeec6f8dcfa575f732e";

        public static string[] GetInProgressOutgoingArgs()
        {
            return new string[]
            {
                CURRENT_SHA,
                TARGET_SHA
            };
        }

        [Fact]
        public static void InProgressArgs()
        {
            var args = new RebaseInProgressArgs(GetInProgressOutgoingArgs());
            Assert.Equal(CURRENT_SHA, args.OriginalTipSha);
            Assert.Equal(TARGET_SHA, args.LandingSha);
        }

        [Fact]
        public static void InProgressArgs_Enumeration()
        {
            var args = new RebaseInProgressArgs(GetInProgressOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(CURRENT_SHA, list[0]);
            Assert.Equal(TARGET_SHA, list[1]);
        }
    }
}
