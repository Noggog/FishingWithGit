using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests.Arguments
{
    public class TakeTests
    {
        public const string ITEM_1 = "Test.txt";
        public const string ITEM_2 = "Test Test.txt";
        public const string ITEM_3 = "TestTest.txt";

        public static string[] GetBasicOutgoingArgs()
        {
            return new string[]
            {
                ITEM_1,
                ITEM_2,
                ITEM_3
            };
        }

        [Fact]
        public static void BasicArgs()
        {
            var args = new TakeArgs(GetBasicOutgoingArgs());
            Assert.Equal(3, args.Items.Count);
            Assert.Equal(ITEM_1, args.Items[0]);
            Assert.Equal(ITEM_2, args.Items[1]);
            Assert.Equal(ITEM_3.ToString(), args.Items[2]);
        }

        public static string[] GetSourceTreeInboundResetArgs()
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

        public const int COMMAND_INDEX = 11;

        public static string[] GetProcessedResetArgs()
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
            var args = new TakeArgs(GetBasicOutgoingArgs());
            var list = args.ToList();
            Assert.Equal(3, list.Count);
            Assert.Equal(ITEM_1, list[0]);
            Assert.Equal(ITEM_2, list[1]);
            Assert.Equal(ITEM_3.ToString(), list[2]);
        }

        [Fact]
        public static void Sourcetree_Reset()
        {
            var args = GetSourceTreeInboundResetArgs().ToList();
            ArgProcessor.Process(CommandType.reset, args);
            Assert.Equal(GetProcessedResetArgs(), args);
        }

        [Fact]
        public static void HookSetCtor_Reset()
        {
            var hook = TakeHooks.Factory(
                null,
                GetSourceTreeInboundResetArgs().ToList(),
                COMMAND_INDEX);
        }

        public static string[] GetSourceTreeInboundCheckoutArgs()
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
                "--",
                "Test File.txt",
                "TestFile.txt",
                "test.txt"
            };
        }

        public static string[] GetProcessedCheckoutArgs()
        {
            return new string[]
            {
                "--no-pager",
                "checkout",
                "--",
                "\"Test File.txt\"",
                "\"TestFile.txt\"",
                "\"test.txt\""
            };
        }

        [Fact]
        public static void Sourcetree_Checkout()
        {
            var args = GetSourceTreeInboundCheckoutArgs().ToList();
            ArgProcessor.Process(CommandType.checkout, args);
            Assert.Equal(GetProcessedCheckoutArgs(), args);
        }

        [Fact]
        public static void HookSetCtor_Checkout()
        {
            var hook = TakeHooks.Factory(
                null,
                GetSourceTreeInboundCheckoutArgs().ToList(),
                COMMAND_INDEX);
        }
    }
}
