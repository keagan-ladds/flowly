using Flowly.Cli.Options;
using System.CommandLine.Invocation;

namespace Flowly.Cli.Handlers
{
    internal abstract class CommandHandler
    {
        public abstract Task HandleAsync(InvocationContext invocationContext);
    }
}
