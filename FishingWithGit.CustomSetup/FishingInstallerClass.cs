using FishingWithGit.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.CustomSetup
{
    [RunInstaller(true)]
    public class FishingInstallerClass : Installer
    {
        private string _targetDir;
        public string TargetDir
        {
            get => _targetDir ?? this.Context.Parameters["targetdir"];
            set => _targetDir = value;
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            AddToPath();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            AddToPath();
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            RemoveFromPath();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            RemoveFromPath();
        }

        public bool Match(string str)
        {
            if (!str.EndsWith("cmd")) return false;
            string targetDir = this.TargetDir.TrimEnd('\\');
            if (!str.Contains(targetDir)) return false;
            return true;
        }

        public void AddToPath()
        {
            RemoveFromPath();
            var pathStr = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);
            var paths = pathStr.Split(';').ToList();

            int index = 0;
            for (; index < paths.Count; index++)
            {
                try
                {
                    var path = paths[index];
                    var dir = new DirectoryInfo(path);
                    if (!dir.Name.Equals("cmd")) continue;
                    if (!dir.Exists) continue;
                    if (IsGitDir(dir))
                    {
                        break;
                    }
                }
                catch (ArgumentException)
                {
                }
            }
            var targetDir = Path.Combine(this.TargetDir.TrimEnd('\\'), "cmd");
            paths.Insert(index, targetDir);

            var toSet = string.Join(";", paths);
            Environment.SetEnvironmentVariable("path", toSet, EnvironmentVariableTarget.Machine);
        }

        private bool IsGitDir(DirectoryInfo dir)
        {
            foreach (var file in dir.EnumerateFiles())
            {
                if (file.Name.Equals("git.exe")) return true;
            }
            return false;
        }

        public void RemoveFromPath()
        {
            var pathStr = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine);
            var paths = pathStr.Split(';').ToList();
            var matchingPaths = paths
                .Where((p) => Match(p))
                .ToList();
            bool removed = false;
            foreach (var match in matchingPaths)
            {
                removed = true;
                paths.Remove(match);
            }
            if (!removed) return;
            var toSet = string.Join(";", paths);
            Environment.SetEnvironmentVariable("path", toSet, EnvironmentVariableTarget.Machine);
        }
    }
}
