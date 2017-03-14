using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class CherryPickTests
    {
        public const string TARGET_SHA = "c5ce43478954719c5577bbeec6f8dcfa575f732e";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                TARGET_SHA
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new CherryPickArgs(GetBasicOutgoingArgs());
            Assert.Equal(TARGET_SHA, args.PickedSha);
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
                "cherry-pick",
                "4787142663ff5e5af0a24dc82b73398470f040a3"
            };
        }

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "cherry-pick",
                "4787142663ff5e5af0a24dc82b73398470f040a3"
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new CherryPickArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(1, list.Count);
            Assert.Equal(TARGET_SHA, list[0]);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.cherry, args);
            Assert.Equal(GetProcessedArgs(), args);
        }
    }
}
