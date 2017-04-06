using FishingWithGit.Tests.Common;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.Tests
{
    public class Repository_Tools
    {
        public const string TYPICAL_SHA = "4787142663ff5e5af0a24dc82b73398470f040a3";
        public const string JUMPBACK_BRANCH = "Jumpback";

        public static RepoCheckout GetTypicalRepo()
        {
            var dir = Utility.GetTemporaryDirectory();
            Repository.Init(dir.FullName);
            var repo = new Repository(dir.FullName);
            var signature = Utility.GetSignature();
            var aFile = new FileInfo(Path.Combine(dir.FullName, Utility.STANDARD_FILE));
            File.WriteAllText(aFile.FullName, "Testing123\n");
            Commands.Stage(repo, aFile.FullName);
            var firstCommit = repo.Commit(
                "First Commit",
                signature,
                signature);
            File.WriteAllText(aFile.FullName, "Testing456\n");
            Commands.Stage(repo, aFile.FullName);
            var secondCommit = repo.Commit(
                "Second Commit",
                signature,
                signature);
            repo.CreateBranch(JUMPBACK_BRANCH);
            File.WriteAllText(aFile.FullName, "Testing789\n");
            Commands.Stage(repo, aFile.FullName);
            var thirdCommit = repo.Commit(
                "Third Commit",
                signature,
                signature);

            return new RepoCheckout(repo);
        }

        public static RepoCheckout GetClone(Repository repo)
        {
            var dir = Utility.GetTemporaryDirectory();
            var repoPath = Repository.Clone(repo.Info.WorkingDirectory, dir.FullName);
            return new RepoCheckout(
                new Repository(repoPath));
        }
    }
}
