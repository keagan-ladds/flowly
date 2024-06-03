using Flowly.Agent.Protocol.Worker;
using Flowly.Core;
using Flowly.Core.Builders;
using Flowly.Core.Definitions;
using Flowly.Core.Providers;

namespace Flowly.Agent
{
    internal class AgentWorker :IWorkerClient
    {
        private readonly IWorkflowRunner _workflowRunner;
        private readonly IExtensionSource _extensionSource;
        private readonly ILoggerSource _loggerSource;
        private readonly Core.Logging.ILogger _logger;

        private Thread _workerThread;
        private CancellationToken _cancellationToken;

        public AgentWorker(IExtensionSource extensionSource, ILoggerSource loggerSource)
        {
            _extensionSource = extensionSource;
            _loggerSource = loggerSource;
            _logger = loggerSource.GetProvider().CreateLogger(nameof(AgentWorker));

            _workflowRunner = BuildRunner();
        }

        private IWorkflowRunner BuildRunner()
        {
            return new RunnerBuilder()
                .WithExtensionSource(_extensionSource)
                .WithLoggerSource(_loggerSource)
                .Build();
        }

        public async Task RunAsync(WorkflowDefinition workflowDefinition)
        {
            if (_workflowRunner == null)
                throw new InvalidOperationException("Workflow Runner has not been defined.");

            await _workflowRunner.RunAsync(workflowDefinition);
        }
    }
}
