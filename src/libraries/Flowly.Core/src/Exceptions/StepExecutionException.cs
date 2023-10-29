using System;

namespace Flowly.Core.Exceptions
{
    internal class StepExecutionException : Exception
    {
        public bool? ContinueOnError { get; set; }
        public bool IsCancelled { get; set; }

        public StepExecutionException(string message, bool isCancelled = false, bool? continueOnError = null)
            : base(message)
        {
            ContinueOnError = continueOnError;
            IsCancelled = isCancelled;
        }
    }
}
