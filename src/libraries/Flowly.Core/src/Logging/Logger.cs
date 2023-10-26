using Flowly.Core.Providers;
using System;

namespace Flowly.Core.Logging
{
    public static class Logger
    {
        internal static ILogger Instance { get; private set; } = new NullLogger();
        public static ILogger GetLoggerInstance(string loggerName) => LoggerProvider?.CreateLogger(loggerName) ?? Instance;
        public static ILoggerProvider? LoggerProvider { get; set; }

        public static void Debug(string message)
        {
            Instance.Debug(message);
        }

        public static void Debug(string message, params object[] args)
        {
            Instance.Debug(message, args);
        }

        public static void Error(string message)
        {
            Instance.Error(message);
        }

        public static void Error(string message, params object[] args)
        {
            Instance.Error(message, args);
        }

        public static void Error(Exception exception, string message)
        {
            Instance.Error(exception, message); 
        }

        public static void Error(Exception exception, string message, params object[] args)
        {
            Instance.Error(exception, message, args);
        }

        public static void Info(string message)
        {
            Instance.Info(message);
        }

        public static void Info(string message, params object[] args)
        {
            Instance.Info(message, args);
        }

        public static void Warn(string message)
        {
            Instance.Warn(message);
        }

        public static void Warn(string message, params object[] args)
        {
            Instance.Warn(message, args);
        }
    }
}
