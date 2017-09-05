using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FishingWithGit.Common
{
    public class Logger
    {
        StringBuilder sb = new StringBuilder();
        bool logFlushed;
        Queue<LogItem> logBuffer = new Queue<LogItem>();
        public bool ConsoleSilent;
        public bool AlwaysLog;
        public double WipeLogsOlderThanDays = -1;
        public readonly string AppName;
        public bool ShouldLogToFile;

        public Logger(string appName)
        {
            this.AppName = appName;
        }

        public void WriteLine(string line, bool error = false, bool? writeToConsole = null)
        {
            WriteLine(new LogItem
            {
                Message = line,
                ToConsole = writeToConsole,
                Date = DateTime.Now,
                Error = error
            });
        }

        public void WriteLine(LogItem item)
        {
            if (logFlushed)
            {
                if (AlwaysLog || (item.ToConsole ?? !ConsoleSilent))
                {
                    if (item.Error)
                    {
                        System.Console.Error.WriteLine(item.Message);
                    }
                    else
                    {
                        System.Console.WriteLine(item.Message);
                    }
                }
                sb.AppendLine($"{item.Date.ToString("MM/dd HH:mm:ss.fff")}: {item.Message}");
            }
            else
            {
                logBuffer.Enqueue(item);
            }
        }

        public void ActivateAndFlushLogging()
        {
            logFlushed = true;
            foreach (var item in logBuffer)
            {
                WriteLine(item);
            }
            logBuffer.Clear();
        }

        public void LogResults(Stopwatch sw, string appName)
        {
            try
            {
                DirectoryInfo curDir = new DirectoryInfo(Directory.GetCurrentDirectory());

                var logDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + $"/Temp/{AppName}/");
                if (!logDir.Exists)
                {
                    logDir.Create();
                }
                var filePath = Path.Combine(logDir.FullName, $"{curDir.Name}.log");

                FileInfo file = new FileInfo(filePath);
                if (file.Exists
                    && this.WipeLogsOlderThanDays >= 0
                    && (DateTime.Now - file.LastWriteTime).TotalDays > this.WipeLogsOlderThanDays)
                {
                    file.Delete();
                }

                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.WriteLine(sb.ToString());
                    writer.WriteLine($"------------------------------------  Elapsed {sw.ElapsedMilliseconds}ms ---------------------------------------------------- {appName} done.");
                }
            }
            catch (Exception)
            {
            }
        }

        public bool LogError(string err, string caption, bool showMessageBox)
        {
            this.WriteLine(err, error: true);
            if (showMessageBox)
            {
                return DialogResult.OK == MessageBox.Show(err, caption, MessageBoxButtons.OKCancel);
            }
            return false;
        }

        public bool LogErrorYesNo(string err, string caption, bool showMessageBox)
        {
            this.WriteLine(err, error: true);
            if (showMessageBox)
            {
                return DialogResult.Yes == MessageBox.Show(err, caption, MessageBoxButtons.YesNo);
            }
            return false;
        }

        public bool? LogErrorRetry(string err, string caption, bool showMessageBox)
        {
            this.WriteLine(err, error: true);
            if (showMessageBox)
            {
                switch (MessageBox.Show(err, caption, MessageBoxButtons.AbortRetryIgnore))
                {
                    case DialogResult.Abort:
                        return false;
                    case DialogResult.Retry:
                        return null;
                    case DialogResult.Ignore:
                        return true;
                    default:
                        throw new NotImplementedException();
                }
            }
            return false;
        }
    }
}
