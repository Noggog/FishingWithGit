using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.Tests.Common
{
    public class Utility
    {
        public const string STANDARD_FILE = "Test.txt";

        public static DirectoryInfo GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "HarmonizeUnitTests", Path.GetRandomFileName()) + "\\";
            Directory.CreateDirectory(tempDirectory);
            return new DirectoryInfo(tempDirectory);
        }

        public static Signature GetSignature()
        {
            var date = new DateTime(2016, 03, 10);
            var signature = new Signature(
                "Justin Swanson",
                "justin.c.swanson@gmail.com",
                date);
            return signature;
        }
    }
}
