using NLog;
using System;

namespace Flowly.Extensions.NLog.Internal
{
    internal class NLogWrapper : Core.Logging.ILogger
    {
        private readonly Logger _loggerInstance;

        public NLogWrapper(Logger loggerInstance)
        {
            _loggerInstance = loggerInstance ?? throw new ArgumentNullException(nameof(loggerInstance));
        }

        public void Debug(string message)
        {
            _loggerInstance.Debug(message);
        }

        public void Debug(string message, params object[] args)
        {
            _loggerInstance.Debug(message, args);
        }

        public void Error(string message)
        {
            _loggerInstance.Error(message);
        }

        public void Error(string message, params object[] args)
        {
            _loggerInstance.Error(message, args);
        }

        public void Error(Exception exception, string message)
        {
            _loggerInstance.Error(exception, message);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            _loggerInstance.Error(exception, message, args);
        }

        public void Info(string message)
        {
            _loggerInstance.Info(message);
        }

        public void Info(string message, params object[] args)
        {
            _loggerInstance.Info(message, args);
        }

        public void Warn(string message)
        {
            _loggerInstance.Warn(message);
        }

        public void Warn(string message, params object[] args)
        {
            _loggerInstance.Warn(message, args);
        }


    }
}
