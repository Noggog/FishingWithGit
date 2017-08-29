using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingWithGit
{
    public abstract class ArgProcessor
    {
        public abstract void Process(List<string> args);

        public static void Process(CommandType commandType, List<string> args)
        {
            StripCArguments(args);
            EnsureFormatIsQuoted(args);
            ProcessAfterSplitterFileList(args);
            AddQuotesToWhitespace(args);
            ArgProcessor commandProcessor;

            switch (commandType)
            {
                case CommandType.clone:
                    commandProcessor = new CloneProcessor();
                    break;
                default:
                    return;
            }
            commandProcessor.Process(args);
        }

        public static void StripCArguments(List<string> args)
        {
            if (!Settings.Instance.CleanCArguments) return;
            int index;
            while ((index = args.IndexOf("-c")) != -1)
            {
                args.RemoveAt(index);
                args.RemoveAt(index);
            }
        }

        public static void EnsureFormatIsQuoted(List<string> args)
        {
            var formatStr = "--format=";
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                if (arg.StartsWith(formatStr))
                {
                    if (arg.Length <= formatStr.Length) continue;
                    if (arg[formatStr.Length + 1] == '\"') continue;
                    if (arg[arg.Length - 1] == '\"') continue;
                    args[i] = $"{formatStr}\"{arg.Substring(formatStr.Length)}\"";
                }
            }
        }

        public static void AddQuotesToWhitespace(List<string> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                if (string.IsNullOrWhiteSpace(arg))
                {
                    args[i] = $"\"{arg}\"";
                }
            }
        }

        public static string AddQuotesIfNecessary(string str)
        {
            if (str.Length == 0) return str;
            if (str[0] != '\"')
            {
                str = "\"" + str;
            }
            if (str[str.Length - 1] != '\"')
            {
                str = str + "\"";
            }
            return str;
        }

        public static void ProcessAfterSplitterFileList(List<string> args, bool throwEx = false)
        {
            var splitterIndex = args.IndexOf("--");
            if (splitterIndex == -1)
            {
                if (throwEx)
                {
                    throw new ArgumentException("Add command had no target files.");
                }
                return;
            }
            for (int i = splitterIndex + 1; i < args.Count; i++)
            {
                args[i] = AddQuotesIfNecessary(args[i]);
            }
        }
    }
}
