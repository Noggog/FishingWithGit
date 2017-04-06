using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.Tests.Common
{
    public class RepoCheckout : IDisposable
    {
        public readonly Repository Repo;
        public readonly DirectoryInfo Dir;

        public RepoCheckout(
            Repository repo)
        {
            this.Repo = repo;
            this.Dir = new DirectoryInfo(repo.Info.WorkingDirectory);
        }

        private void DeleteDirectory(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public void Dispose()
        {
            this.Repo.Dispose();
            DeleteDirectory(this.Dir.FullName);
        }
    }

}
