using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class PullTests
    {
        const string CURRENT_SHA = "c04d6db5d07394a7e0ec4db2e3035fc5477af471";
        const string TARGET_SHA = "55e8c38ef98d046d499f22e80ada6689295ad7f4";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                CURRENT_SHA,
                TARGET_SHA
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new PullArgs(GetBasicOutgoingArgs());
            Assert.Equal(CURRENT_SHA, args.CurrentSha);
            Assert.Equal(TARGET_SHA, args.TargetSha);
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
                "pull",
                "origin",
                "master"
            };
        }

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "pull",
                "origin",
                "master"
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new PullArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(CURRENT_SHA, list[0]);
            Assert.Equal(TARGET_SHA, list[1]);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.commit, args);
            Assert.Equal(GetProcessedArgs(), args);
        }
    }
}
