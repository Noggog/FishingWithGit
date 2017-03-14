using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class CheckoutTests
    {
        public const string CURRENT_SHA = "92d1ea36962347d2f072d3d67a5b4842a0d9cf74";
        public const string TARGET_SHA = "c5ce43478954719c5577bbeec6f8dcfa575f732e";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                CURRENT_SHA,
                TARGET_SHA
            };
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
                "checkout",
                "TestBranch"
            };
        }

        public static string[] GetProcessedArgs()
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
                "checkout",
                "TestBranch"
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new CheckoutArgs(GetBasicOutgoingArgs());
            Assert.Equal(CURRENT_SHA, args.CurrentSha);
            Assert.Equal(TARGET_SHA, args.TargetSha);
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new CheckoutArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal(CURRENT_SHA, list[0]);
            Assert.Equal(TARGET_SHA, list[1]);
        }

        [Fact]
        public static void Sourcetree()
        {
            //var arg = "--no-pager -c color.branch=false -c color.diff=false -c color.status=false -c diff.mnemonicprefix=false -c core.quotepath=false checkout master";
            //var args =
        }
    }
}
