using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class CommitMsgTests
    {
        public const string PATH = "C:\\Users\\Noggog\\AppData\\Local\\Temp\\scqnivtj.bbu";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                PATH
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new CommitMsgArgs(GetBasicOutgoingArgs());
            Assert.Equal(PATH, args.CommitMessageFilePath);
        }

        [Fact]
        public static void BasicArgs_Enumeration()
        {
            var args = new CommitMsgArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(1, list.Count);
            Assert.Equal(PATH, list[0]);
        }
    }
}
