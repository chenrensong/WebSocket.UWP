using System;

namespace WebSocket4UWP.Internal
{
    internal interface ICanLog { }

    public enum LogLevel
    {
        None,
        Trace,
        Debug,
        Info,
        Warn,
        Error
    }

    internal static class LogExtensions
    {
        public static void LogTrace(this ICanLog canLog, string fmt, params object[] args)
        {
            LogMessage(canLog, LogLevel.Trace, fmt, args);
        }

        public static void LogDebug(this ICanLog canLog, string fmt, params object[] args)
        {
            LogMessage(canLog, LogLevel.Debug, fmt, args);
        }

        public static void LogInfo(this ICanLog canLog, string fmt, params object[] args)
        {
            LogMessage(canLog, LogLevel.Info, fmt, args);
        }

        public static void LogWarning(this ICanLog canLog, string fmt, params object[] args)
        {
            LogMessage(canLog, LogLevel.Warn, fmt, args);
        }

        public static void LogError(this ICanLog canLog, string fmt, params object[] args)
        {
            LogMessage(canLog, LogLevel.Error, fmt, args);
        }

        public static void LogError(this ICanLog canLog, Exception e)
        {
            LogMessage(canLog, LogLevel.Error, "{0}", e);
        }

        public static void LogError(this ICanLog canLog, string message, Exception e)
        {
            LogMessage(canLog, LogLevel.Error, message + " {0}", e);
        }

        private static void LogMessage(ICanLog canLog, LogLevel logLevel, string fmt, params object[] args)
        {
            var message = args.Length > 0 ? string.Format(fmt, args) : fmt;
            LogManager.Instance.LogMessage(canLog.GetType(), logLevel, message);
        }
    }
}
