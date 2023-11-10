using Flowly.Core.Providers;

namespace Flowly.Core.Logging
{
    public class NullLoggerSource : ILoggerSource
    {
        public ILoggerProvider GetProvider()
        {
            return new NullLoggerProvider();
        }
    }

    internal class NullLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string loggerName)
        {
            return new NullLogger();    
        }
    }
}
