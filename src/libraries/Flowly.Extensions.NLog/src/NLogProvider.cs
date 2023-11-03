using Flowly.Core.Providers;
using Flowly.Extensions.NLog.Internal;
using NLog;
using System;

namespace Flowly.Extensions.NLog
{
    public class NLogProvider : ILoggerProvider
    {
        private readonly LogFactory _logFactory;

        public NLogProvider(LogFactory logFactory)
        {
            _logFactory = logFactory ?? throw new ArgumentNullException(nameof(logFactory));
        }

        public Core.Logging.ILogger CreateLogger(string loggerName)
        {
            var logger = _logFactory.GetLogger(loggerName);
            return new NLogWrapper(logger);
        }
    }
}
