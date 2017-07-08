using FishingWithGit.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class PushTests
    {
        const string MASTER_REF = "master:master";
        const string REMOTE = "origin";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                REMOTE,
                MASTER_REF
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new PushArgs(GetBasicOutgoingArgs());
            Assert.Equal(REMOTE, args.Remote);
            Assert.Equal(1, args.RefSpecs.Count);
            Assert.Equal(MASTER_REF, string.Join(":", args.RefSpecs[0].Item1, args.RefSpecs[0].Item2));
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
                "-c",
                "credential.helper=manager-st",
                "push",
                "-v",
                REMOTE,
                MASTER_REF,
                Repository_Tools.JUMPBACK_BRANCH
            };
        }

        public const int COMMAND_INDEX = 13;

        public static string[] GetProcessedArgs()
        {
            return new string[]
            {
                "--no-pager",
                "push",
                "-v",
                REMOTE,
                MASTER_REF,
                Repository_Tools.JUMPBACK_BRANCH
            };
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new PushArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(GetBasicOutgoingArgs().Length, list.Count);
        }

        [Fact]
        public static void Sourcetree()
        {
            var args = GetSourceTreeInboundArgs().ToList();
            ArgProcessor.Process(CommandType.push, args);
            Assert.Equal(GetProcessedArgs(), args);
        }

        [Fact]
        public static void HookSetCtor()
        {
            var hook = PushHooks.Factory(
                null,
                GetProcessedArgs().ToList(),
                COMMAND_INDEX);
        }
    }
}
