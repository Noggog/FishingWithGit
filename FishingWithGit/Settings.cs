using FishingWithGit.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FishingWithGit
{
    public class Settings : AbstractSettings
    {
        private static Settings _settings;
        public static Settings Instance => GetSettings();
        public const string XMLNamespace = "http://tempuri.org/FishingWithGitSettings.xsd";

        public string RealGitProgramPath = string.Empty;
        public bool ShouldLog = true;
        public byte WipeLogsOlderThanDays = 1;
        public bool FireHookLogic = true;
        public bool CleanCArguments = true;
        public bool RemoveTemplateFromClone = true;
        public int ProcessTimeoutWarning = 10000;
        public bool PrintSeparateArgs = false;
        public bool RunMassHooks = true;
        public string MassHookFolder = @"..\Mass Hooks\";
        public string RealGitProgramPathOverride = string.Empty;
        public bool RunInRepoHooks = false;
        public bool RunNormalFolderHooks = true;
        public string PathLoadedFrom;

        Settings()
        {
        }

        private static Settings GetSettings()
        {
            if (_settings == null)
            {
                _settings = CreateSettings();
            }
            return _settings;
        }

        private static Settings CreateSettings()
        {
            if (!GetSettingsRelativeToExe("FishingWithGitSettings.xml", out var file)
                || !file.Exists)
            {
                return new Settings()
                {
                    PathLoadedFrom = file.FullName
                };
            }

            var xml = GetXML(file);
            
            var ret = new Settings()
            {
                PathLoadedFrom = file.FullName,
                RealGitProgramPath = GetString(xml, XName.Get(nameof(RealGitProgramPath), XMLNamespace), string.Empty),
                ShouldLog = GetBool(xml, XName.Get(nameof(ShouldLog), XMLNamespace), true),
                WipeLogsOlderThanDays = GetByte(xml, XName.Get(nameof(WipeLogsOlderThanDays), XMLNamespace), 1),
                FireHookLogic = GetBool(xml, XName.Get(nameof(FireHookLogic), XMLNamespace), true),
                CleanCArguments = GetBool(xml, XName.Get(nameof(CleanCArguments), XMLNamespace), true),
                RemoveTemplateFromClone = GetBool(xml, XName.Get(nameof(RemoveTemplateFromClone), XMLNamespace), true),
                ProcessTimeoutWarning = GetInt(xml, XName.Get(nameof(ProcessTimeoutWarning), XMLNamespace), 10000),
                PrintSeparateArgs = GetBool(xml, XName.Get(nameof(PrintSeparateArgs), XMLNamespace), false),
                RunMassHooks = GetBool(xml, XName.Get(nameof(RunMassHooks), XMLNamespace), true),
                RealGitProgramPathOverride = GetString(xml, XName.Get(nameof(RealGitProgramPathOverride), XMLNamespace), string.Empty),
                RunInRepoHooks = GetBool(xml, XName.Get(nameof(RunInRepoHooks), XMLNamespace), false),
                RunNormalFolderHooks = GetBool(xml, XName.Get(nameof(RunNormalFolderHooks), XMLNamespace), true),
            };

            return ret;
        }

        public bool SaveSettings()
        {
            if (!GetSettingsRelativeToExe("FishingWithGitSettings.xml", out var file))
            {
                return false;
            }

            this.PathLoadedFrom = file.FullName;
            XDocument doc = new XDocument();
            doc.Add(new XElement(XName.Get("FishingWithGitSettings", XMLNamespace)));
            doc.Root.Add(new XElement(XName.Get(nameof(RealGitProgramPath), XMLNamespace), new XAttribute(VALUE, this.RealGitProgramPath)));
            doc.Root.Add(new XElement(XName.Get(nameof(ShouldLog), XMLNamespace), new XAttribute(VALUE, this.ShouldLog)));
            doc.Root.Add(new XElement(XName.Get(nameof(WipeLogsOlderThanDays), XMLNamespace), new XAttribute(VALUE, this.WipeLogsOlderThanDays)));
            doc.Root.Add(new XElement(XName.Get(nameof(FireHookLogic), XMLNamespace), new XAttribute(VALUE, this.FireHookLogic)));
            doc.Root.Add(new XElement(XName.Get(nameof(CleanCArguments), XMLNamespace), new XAttribute(VALUE, this.CleanCArguments)));
            doc.Root.Add(new XElement(XName.Get(nameof(RemoveTemplateFromClone), XMLNamespace), new XAttribute(VALUE, this.RemoveTemplateFromClone)));
            doc.Root.Add(new XElement(XName.Get(nameof(ProcessTimeoutWarning), XMLNamespace), new XAttribute(VALUE, this.ProcessTimeoutWarning)));
            doc.Root.Add(new XElement(XName.Get(nameof(PrintSeparateArgs), XMLNamespace), new XAttribute(VALUE, this.PrintSeparateArgs)));
            doc.Root.Add(new XElement(XName.Get(nameof(RunMassHooks), XMLNamespace), new XAttribute(VALUE, this.RunMassHooks)));
            doc.Root.Add(new XElement(XName.Get(nameof(RealGitProgramPathOverride), XMLNamespace), new XAttribute(VALUE, this.RealGitProgramPathOverride)));
            doc.Root.Add(new XElement(XName.Get(nameof(RunInRepoHooks), XMLNamespace), new XAttribute(VALUE, this.RunInRepoHooks)));
            doc.Root.Add(new XElement(XName.Get(nameof(RunNormalFolderHooks), XMLNamespace), new XAttribute(VALUE, this.RunNormalFolderHooks)));
            try
            {
                doc.Save(file.FullName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
