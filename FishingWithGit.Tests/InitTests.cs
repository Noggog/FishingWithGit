using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests
{
    public class InitTests
    {
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
    }
}
