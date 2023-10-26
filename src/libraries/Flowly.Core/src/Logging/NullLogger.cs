using System;

namespace Flowly.Core.Logging
{
    internal class NullLogger : ILogger
    {
        public void Debug(string message) { }

        public void Debug(string message, params object[] args) { }

        public void Error(string message) { }

        public void Error(string message, params object[] args) { }

        public void Error(Exception exception, string message) { }

        public void Error(Exception exception, string message, params object[] args) { }

        public void Info(string message) { }

        public void Info(string message, params object[] args) { }

        public void Warn(string message) { }

        public void Warn(string message, params object[] args) { }
    }
}
