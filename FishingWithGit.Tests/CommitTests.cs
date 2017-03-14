using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class CommitTests
    {
        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new CommitArgs(GetBasicOutgoingArgs());
            Assert.False(args.Amending);
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
                "commit",
                "-q",
                "-F",
                "C:\\Users\\Noggog\\AppData\\Local\\Temp\\scqnivtj.bbu"
            };
        }

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "commit",
                "-q",
                "-F",
                "C:\\Users\\Noggog\\AppData\\Local\\Temp\\scqnivtj.bbu"
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new CommitArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(0, list.Count);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.commit, args);
            Assert.Equal(GetProcessedArgs(), args);
        }

        public static string[] GetAmendOutgoingArgs()
        {
            return new string[]
            {
                "--amend"
            };
        }

        public static string[] GetSourceTreeAmendArgs()
        {
            return new string[]
            {
                "--no-pager",
                "commit",
                "--amend",
                "-q",
                "-F",
                "C:\\Users\\Noggog\\AppData\\Local\\Temp\\scqnivtj.bbu"
            };
        }

        [Fact]
        public static void AmendingArgs()
        {
            var args = new CommitArgs(GetAmendOutgoingArgs());
            Assert.True(args.Amending);
        }

        [Fact]
        public static void AmendingArgs_Enumeration()
        {
            var args = new CommitArgs(GetSourceTreeAmendArgs());
            var list = args.ToList();
            Assert.Equal(1, list.Count);
            Assert.Equal("--amend", list[0]);
        }
    }
}
