using Flowly.Core.Providers;
using NLog;

namespace Flowly.Logging.NLog
{
    public class NLogSource : ILoggerSource
    {
        public LogFactory? LogFactory { get; set; }

        public ILoggerProvider GetProvider()
        {
            return new NLogProvider(LogFactory ?? LogManager.LogFactory);
        }
    }
}
