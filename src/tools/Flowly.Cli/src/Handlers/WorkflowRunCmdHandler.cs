using Flowly.Cli.Extensions;
using Flowly.Cli.Options;
using Flowly.Core.Builders;
using Flowly.Core.Providers;

namespace Flowly.Cli.Handlers
{
    internal class WorkflowRunCmdHandler 
    {
        private readonly ILoggerSource _loggerSource;
        public WorkflowRunCmdHandler(ILoggerSource loggerSource)
        {
            _loggerSource = loggerSource ?? throw new ArgumentNullException(nameof(loggerSource));
        }

        public Task HandleAsync(WorkflowRunCmdOptions options)
        {
            var workflow = new WorkflowBuilder()
                .FromOptions(options)
                 .Build();

            var runner = new RunnerBuilder()
                .WithLoggerSource(_loggerSource)
                .FromOptions(options)
                .Build();

            return runner.RunAsync(workflow);
        }
    }
}
