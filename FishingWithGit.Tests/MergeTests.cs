using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class MergeTests
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
            var args = new MergeArgs(GetBasicOutgoingArgs());
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
                "merge",
                "master",
            };
        }

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "merge",
                "master",
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new MergeArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(1, list.Count);
            Assert.Equal(TARGET_BRANCH, list[0]);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.merge, args);
            Assert.Equal(GetProcessedArgs(), args);
        }
    }
}
