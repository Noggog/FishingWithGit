using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class ResetTests
    {
        public const string TARGET_BRANCH = "master";
        public const string TARGET_SHA = "c5ce43478954719c5577bbeec6f8dcfa575f732e";
        public const ResetType TYPE = ResetType.hard;

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                TARGET_BRANCH,
                TARGET_SHA,
                TYPE.ToString()
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new ResetArgs(GetBasicOutgoingArgs());
            Assert.Equal(TARGET_BRANCH, args.Branch);
            Assert.Equal(TARGET_SHA, args.TargetSha);
            Assert.Equal(TYPE, args.Type);
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
                "reset",
                "-q",
                "--hard",
                "80755d018ecad38e5ea7db462d33a55de4c062cc"
            };
        }

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "reset",
                "-q",
                "--hard",
                "80755d018ecad38e5ea7db462d33a55de4c062cc"
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new ResetArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(3, list.Count);
            Assert.Equal(TARGET_BRANCH, list[0]);
            Assert.Equal(TARGET_SHA, list[1]);
            Assert.Equal(TYPE.ToString(), list[2]);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.reset, args);
            Assert.Equal(GetProcessedArgs(), args);
        }
    }
}
