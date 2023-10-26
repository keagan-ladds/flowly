using Flowly.Core.Logging;

namespace Flowly.Core.Providers
{
    public interface ILoggerProvider
    {
        ILogger CreateLogger(string loggerName);
    }
}
