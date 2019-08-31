using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit.Common
{
    public interface ILogger
    {
        void WriteLine(string line, bool error = false, bool? writeToConsole = null);
        void WriteLine(LogItem item);
        bool LogError(string err, string caption, bool showMessageBox);
        bool LogErrorYesNo(string err, string caption, bool showMessageBox);
        bool? LogErrorRetry(string err, string caption, bool showMessageBox);
    }
}
