using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void WriteLine(string line, bool error = false, bool? writeToConsole = false)
        {
            WriteLine(new LogItem
            {
                Message = line,
                ToConsole = writeToConsole,
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
                sb.AppendLine(item.Message);
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

        public void LogResults()
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
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
