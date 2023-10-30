using Flowly.Core.Providers;
using System;

namespace Flowly.Core.Logging
{
    public static class Logger
    {
        public static ILogger GetLoggerInstance(string loggerName) => LoggerProvider?.CreateLogger(loggerName) ?? new NullLogger();
        public static ILoggerProvider? LoggerProvider { get; set; }
    }
}
