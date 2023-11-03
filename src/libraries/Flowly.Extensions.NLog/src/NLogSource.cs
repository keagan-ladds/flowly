using Flowly.Core.Providers;
using NLog;

namespace Flowly.Extensions.NLog
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
