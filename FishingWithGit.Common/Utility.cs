using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.Common
{
    public class Utility
    {
        public static bool TestIfFishingEXE(string path)
        {
            using (var process = Process.Start(
                new ProcessStartInfo()
                {
                    FileName = path,
                    Arguments = Constants.IS_FISHING_CMD,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    UseShellExecute = false
                }))
            {
                process.WaitForExit();
                using (StreamReader reader = process.StandardOutput)
                {
                    var str = reader.ReadToEnd();
                    if (str.Trim().Equals(Constants.IS_FISHING_RESP)) return true;
                }
            }
            return false;
        }
    }
}
