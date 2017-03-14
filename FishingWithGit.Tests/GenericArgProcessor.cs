using FishingWithGit.Tests.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FishingWithGit.Tests
{
    public class GenericArgProcessor
    {
        public List<string> GetFormatArgs()
        {
            return new List<string>()
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
                "log",
                "--max-count=200",
                "--branches",
                "--tags",
                "--remotes",
                "--full-history",
                "--format=%H%h%P%ci%cn <%ce>%an <%ae>%d%s",
                "--decorate=full",
                "--date=iso",
                "HEAD",
                "--"
            };
        }

        public List<string> GetCheckoutResetArgs()
        {
            return new List<string>()
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

        [Fact]
        public void StripCArgument_Test()
        {
            var args = GetFormatArgs();
            ArgProcessor.StripCArguments(args);
            var newArgs = new string[]
            {
                "--no-pager",
                "log",
                "--max-count=200",
                "--branches",
                "--tags",
                "--remotes",
                "--full-history",
                "--format=%H%h%P%ci%cn <%ce>%an <%ae>%d%s",
                "--decorate=full",
                "--date=iso",
                "HEAD",
                "--"
            };
            Assert.Equal(newArgs, args);
        }

        [Fact]
        public void EnsureFormatIsQuoted_Test()
        {
            var args = GetFormatArgs();
            ArgProcessor.EnsureFormatIsQuoted(args);
            var newArgs = new string[]
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
                "log",
                "--max-count=200",
                "--branches",
                "--tags",
                "--remotes",
                "--full-history",
                "--format=\"%H%h%P%ci%cn <%ce>%an <%ae>%d%s\"",
                "--decorate=full",
                "--date=iso",
                "HEAD",
                "--"
            };
            Assert.Equal(newArgs, args);
        }

        [Fact]
        public void AddQuotesIfNecessary_Test()
        {
            var str = "Test File.txt";
            var output = ArgProcessor.AddQuotesIfNecessary(str);
            Assert.Equal("\"Test File.txt\"", output);
        }

        [Fact]
        public void AddQuotesIfNecessary_NoAdd_Test()
        {
            var str = "\"Test File.txt\"";
            var output = ArgProcessor.AddQuotesIfNecessary(str);
            Assert.Equal("\"Test File.txt\"", output);
        }

        [Fact]
        public void ProcessAfterSplitterFileList_Test()
        {
            var args = GetCheckoutResetArgs();
            ArgProcessor.ProcessAfterSplitterFileList(args);
            var newArgs = new string[]
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
                "\"Test File.txt\"",
                "\"TestFile.txt\"",
                "\"test.txt\""
            };
            Assert.Equal(newArgs, args);
        }
    }
}
