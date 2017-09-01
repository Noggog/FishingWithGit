using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FishingWithGit.Common
{
    public abstract class AbstractSettings
    {
        public const string VALUE = "value";

        public static bool GetBool(XElement elem, XName name, bool def)
        {
            var attr = elem.Element(name)?.Attribute(VALUE);
            if (attr == null) return def;
            if (!bool.TryParse(attr.Value, out var val)) return def;
            return val;
        }

        public static int GetInt(XElement elem, XName name, int def)
        {
            var attr = elem.Element(name)?.Attribute(VALUE);
            if (attr == null) return def;
            if (!int.TryParse(attr.Value, out var val)) return def;
            return val;
        }

        public static string GetString(XElement elem, XName name, string def)
        {
            var attr = elem.Element(name)?.Attribute(VALUE);
            if (attr == null) return def;
            return attr.Value;
        }

        public static byte GetByte(XElement elem, XName name, byte def)
        {
            var attr = elem.Element(name)?.Attribute(VALUE);
            if (attr == null) return def;
            if (!byte.TryParse(attr.Value, out var val)) return def;
            return val;
        }

        public static bool GetSettingsRelativeToExe(string path, out FileInfo file)
        {
            var assemb = System.Reflection.Assembly.GetEntryAssembly();
            if (assemb == null)
            {
                file = default(FileInfo);
                return false;
            }
            var exe = new FileInfo(assemb.Location);
            var fullPath = Path.Combine(exe.Directory.FullName, path);
            file = new FileInfo(fullPath);
            return true;
        }

        public static XElement GetXML(FileInfo file)
        {
            XDocument xml;
            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new StreamReader(stream))
                {
                    xml = XDocument.Parse(reader.ReadToEnd());
                }
            }
            return xml.Root;
        }
    }
}
