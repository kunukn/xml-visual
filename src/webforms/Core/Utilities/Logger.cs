using System;
using System.Diagnostics;


namespace Kunukn.XmlVisual.Core.Utilities
{
    /// <summary>
    /// Kunuk Nykjaer
    /// </summary>
    public static class Logger
    {
        public static void LogInfo(string title, string msg)
        {
            EventLog.WriteEntry(Trim(title), msg, EventLogEntryType.Information);
        }
        public static void LogWarning(string title, string msg)
        {
            EventLog.WriteEntry(Trim(title), msg, EventLogEntryType.Warning);
        }
        public static void LogError(string title, string msg)
        {
            EventLog.WriteEntry(Trim(title), msg, EventLogEntryType.Error);
        }

        private static string Trim(string s)
        {
            string str = s;
            if (str.Length > 20)
                str = str.Substring(0, 20);
            return str;
        }
    }
}
